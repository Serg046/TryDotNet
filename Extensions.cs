using InterviewDotNet.Services;
using SpawnDev.BlazorJS.WebWorkers;

namespace InterviewDotNet;

internal static class Extensions
{
    public static Task<TReturn> InvokeAsync<TService, TReturn>(this IWebWorkerService webWorkerService, string methodName, params object?[]? args)
    {
        return InvokeAsync<TService, TReturn>(webWorkerService.WebWorker, methodName, args);
    }

    public static async Task<TReturn> InvokeAsync<TService, TReturn>(this Task<WebWorker> webWorker, string methodName, params object?[]? args)
    {
        var worker = await webWorker;
        return await worker.InvokeAsync<TService, TReturn>(methodName, args);
    }
}
