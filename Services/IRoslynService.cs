using InterviewDotNet.Models;

namespace InterviewDotNet.Services;

public interface IRoslynService
{
    Task<string> CompileAndRun(string code);
    Task<IReadOnlyList<Completion>> GetCompletions(string code, CompletionRequest request);
}