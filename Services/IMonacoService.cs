namespace InterviewDotNet.Services
{
    public interface IMonacoService
    {
        Task<MonacoService.Completion[]> GetCompletionAsync(string code, MonacoService.CompletionRequest completionRequest);
    }
}