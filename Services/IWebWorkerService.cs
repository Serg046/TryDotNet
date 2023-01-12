using SpawnDev.BlazorJS.WebWorkers;

namespace InterviewDotNet.Services;

public interface IWebWorkerService
{
    Task<WebWorker> WebWorker { get; }
    Task InitAsync();
    Task<WebWorker?> GetWebWorker(bool verboseMode = false, bool awaitWhenReady = true);
}
