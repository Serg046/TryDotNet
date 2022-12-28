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

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        try
        {
            var compilation = CSharpCompilation.Create("Fiddle", new[] { syntaxTree }, await _references.Value);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            new SyntaxWalker(completionRequest).Visit(syntaxTree.GetRoot());
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

        public SyntaxWalker(CompletionRequest request)
        {
            _request = request;
        }

        public override void Visit(SyntaxNode? node)
        {
            base.Visit(node);
            var span = node?.SyntaxTree.GetLineSpan(node.Span);
            var lineNumber = span?.StartLinePosition.Line;
            if (lineNumber == _request.Line)
            {
                Console.WriteLine(span.ToString());
            }
        }
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
