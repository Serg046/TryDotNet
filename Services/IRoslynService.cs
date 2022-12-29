namespace InterviewDotNet.Services;

public interface IRoslynService
{
    Task<string> CompileAndRun(string code);
}