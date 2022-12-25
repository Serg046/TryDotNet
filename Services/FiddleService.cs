using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace InterviewDotNet.Services;

public class FiddleService
{
    private static Lazy<Task<IReadOnlyList<MetadataReference>>>? _references;

    /*public FiddleService(HttpClient httpClient)
    {
        _references ??= new(async () =>
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Console.dll")),
                MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Private.CoreLib.dll")),
                MetadataReference.CreateFromStream(await httpClient.GetStreamAsync("_framework/System.Runtime.dll"))
            };
            return references;
        });
    }*/

    public Task<string> Run(string code)
    {
        return Task.FromResult("ololo");
    }

    public async Task<string> Run2(string code)
    {
        if (_references == null) throw new MissingMemberException("Cannot find the references");

        string output;
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create("Fiddle", new[] { syntaxTree }, await _references.Value, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (diagnostics.Count == 0)
        {
            using var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);
            stream.Position = 0;

            if (emitResult.Success)
            {
                output = LoadAndRun(stream);
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

    private static string LoadAndRun(Stream assembly)
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
            methodInfo.Invoke(instance, null);
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

    private string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics) => string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString()));

    private class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true)
        {
        }
    }
}
