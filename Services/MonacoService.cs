using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;

namespace InterviewDotNet.Services;

public class MonacoService : IMonacoService
{
    private static Lazy<Task<IReadOnlyList<MetadataReference>>>? _references;

    public MonacoService()
    {
        var httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost:5005/") };
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
    }

    [JSInvokable]
    public async Task<Completion[]> GetCompletionAsync(string code, CompletionRequest completionRequest)
    {
        if (_references == null) throw new MissingMemberException("Cannot find the references");
        Console.WriteLine($"Column: {completionRequest.Column}, line: {completionRequest.Line}");

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        try
        {
            var compilation = CSharpCompilation.Create("Fiddle", new[] { syntaxTree }, await _references.Value);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var walker = new SyntaxWalker(completionRequest, semanticModel);
            walker.Visit(syntaxTree.GetRoot());
            Console.WriteLine(walker.Completions.Single());
            var y = semanticModel.LookupSymbols(completionRequest.Column, walker.Completions.Single());
            foreach (var z in y)
            {
                Console.WriteLine($"{z} {z.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        //var memberAccessNode = (MemberAccessExpressionSyntax)syntaxTree.GetRoot().DescendantNodes(dotTextSpan).Last();
        return new[]
        {
            new Completion { Label = "Label", InsertText = "InsertText", Kind = CompletionItemKind.Text, Documentation = "Doc" }
        };
    }

    class SyntaxWalker : CSharpSyntaxWalker
    {
        private readonly CompletionRequest _request;
        private readonly SemanticModel _semanticModel;
        private readonly List<CompletionItem> _completions = new();

        public SyntaxWalker(CompletionRequest request, SemanticModel semanticModel)
        {
            _request = request;
            _semanticModel = semanticModel;
        }

        public IReadOnlyList<ITypeSymbol> Completions
        {
            get
            {
                var x = _completions.SingleOrDefault(c => c.Column == _request.Column) ?? _completions.Single();
                return new[] { x.Completion };
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
                if (line == _request.Line && (column == _request.Column || column == _request.Column - 1))
                {
                    var typeSymbol = _semanticModel.GetTypeInfo(node).Type;
                    if (typeSymbol != null)
                    {
                        _completions.Add(new(column, typeSymbol));
                    }
                }
            }
        }

        private record CompletionItem(int Column, ITypeSymbol Completion);
    }

    public class CompletionRequest
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class Completion
    {
        public string Label { get; init; } = "";
        public string InsertText { get; init; } = "";
        public CompletionItemKind Kind { get; init; }
        public string Documentation { get; init; } = "";
    }

    public enum CompletionItemKind : int
    {
        Method = 0,
        Function = 1,
        Constructor = 2,
        Field = 3,
        Variable = 4,
        Class = 5,
        Struct = 6,
        Interface = 7,
        Module = 8,
        Property = 9,
        Event = 10,
        Operator = 11,
        Unit = 12,
        Value = 13,
        Constant = 14,
        Enum = 15,
        EnumMember = 16,
        Keyword = 17,
        Text = 18,
        Color = 19,
        File = 20,
        Reference = 21,
        Customcolor = 22,
        Folder = 23,
        TypeParameter = 24,
        User = 25,
        Issue = 26,
        Snippet = 27
    }
}
