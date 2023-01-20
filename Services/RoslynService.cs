using InterviewDotNet.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.JSInterop;
using System.Runtime.Loader;

namespace InterviewDotNet.Services;

public class RoslynService : IRoslynService
{
    private static Lazy<Task<IReadOnlyList<MetadataReference>>>? _references;

    public RoslynService(HttpClient httpClient)
    {
        _references ??= new(async () => new List<MetadataReference>
        {
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Console.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Private.CoreLib.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Runtime.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Linq.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Linq.Expressions.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Collections.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Collections.Concurrent.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.ValueTuple.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Timer.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.ThreadPool.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Thread.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Tasks.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Tasks.Parallel.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Tasks.Extensions.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Tasks.Dataflow.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Overlapped.dll")),
            MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Threading.Channels.dll"))
        });
    }

    public async Task<string> CompileAndRun(string code)
    {
        var (_, compilation) = await Compile(code);
        var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        string output;
        if (diagnostics.Count == 0)
        {
            using var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);
            stream.Position = 0;

            if (emitResult.Success)
            {
                output = await LoadAndRun(stream);
            }
            else
            {
                output = FormatDiagnostics(emitResult.Diagnostics);
            }
        }
        else
        {
            output = FormatDiagnostics(diagnostics);
        }

        return output;
    }

    private static async Task<(SyntaxTree SyntaxTree, CSharpCompilation Compilation)> Compile(string code)
    {
        if (_references == null) throw new MissingMemberException("Cannot find the references");
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create("Fiddle", new[] { syntaxTree }, await _references.Value, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return (syntaxTree, compilation);
    }

    private static async Task<string> LoadAndRun(Stream assembly)
    {
        string output;
        try
        {
            var host = new CollectibleAssemblyLoadContext();
            var assemby = host.LoadFromStream(assembly);
            var type = assemby.GetType("Program") ?? throw new MissingMemberException("Program class was not found");
            var methodInfo = type.GetMethod("Main") ?? throw new MissingMethodException("Program", "Main");
            var instance = Activator.CreateInstance(type);
            var consoleOut = new StringWriter();
            Console.SetOut(consoleOut);
            if (methodInfo.Invoke(instance, null) is Task task)
            {
                await task;
            }
            Console.SetOut(Console.Out);
            output = consoleOut.ToString();
            host.Unload();
        }
        catch (Exception ex)
        {
            output = ex.ToString();
        }

        return output;
    }

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics) => string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString()));

    [JSInvokable]
    public async Task<IReadOnlyList<Completion>> GetCompletions(string code, CompletionRequest request)
    {
        var (syntaxTree, compilation) = await Compile(code);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var visitor = new SyntaxNodeVisitor(request, semanticModel);
        visitor.Visit(syntaxTree.GetRoot());
        return visitor.GetCompletions().Select(c => c.Name).Distinct().Select(symbolName => new Completion(symbolName, symbolName)).ToList();
    }

    private class SyntaxNodeVisitor : CSharpSyntaxWalker
    {
        private readonly CompletionRequest _request;
        private readonly SemanticModel _semanticModel;
        private readonly List<CompletionItem> _completionItems = new();

        public SyntaxNodeVisitor(CompletionRequest request, SemanticModel semanticModel)
        {
            _request = request;
            _semanticModel = semanticModel;
        }

        public IEnumerable<ISymbol> GetCompletions()
        {
            var noDotCompletionItems = _completionItems.Where(c => c.Column == _request.Column).ToList();
            var completionItems = noDotCompletionItems.Count > 0 ? noDotCompletionItems : _completionItems;
            foreach (var (column, typeSymbol) in completionItems)
            foreach (var symbol in _semanticModel.LookupSymbols(column, typeSymbol))
            {
                yield return symbol;
            }
        }

        public override void Visit(SyntaxNode? node)
        {
            base.Visit(node);
            if (node != null)
            {
                var span = node.SyntaxTree.GetLineSpan(node.Span);
                var line = span.EndLinePosition.Line;
                var column = span.EndLinePosition.Character;
                // The current symbol might be a dot that's why we also include the previous one 
                if (line == _request.Line && (column == _request.Column || column == _request.Column - 1))
                {
                    var typeSymbol = _semanticModel.GetTypeInfo(node).Type;
                    if (typeSymbol != null)
                    {
                        _completionItems.Add(new(column, typeSymbol));
                    }
                }
            }
        }

        private record CompletionItem(int Column, ITypeSymbol TypeSymbol);
    }

    private class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true)
        {
        }
    }
}
