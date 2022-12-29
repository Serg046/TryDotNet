namespace InterviewDotNet.Models;

public record Completion(string Label, string InsertText, CompletionItemKind Kind = CompletionItemKind.Text);
