using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.WebWorkers;
using System.Diagnostics.CodeAnalysis;

namespace InterviewDotNet.Services;

public class WebWorkerService : SpawnDev.BlazorJS.WebWorkers.WebWorkerService, IWebWorkerService
{
    [AllowNull] // InitAsync is called from Program.cs at startup 
    private Task _appLevelInitialization;

    public WebWorkerService(NavigationManager navigator, HttpClient httpClient, IRoslynService roslynService) : base(navigator, httpClient, new ServiceProvider(roslynService))
    {
    }

    [AllowNull]
    public Task<WebWorker> WebWorker { get; private set; }

    Task IWebWorkerService.InitAsync() => InitAsync();
    public async new Task InitAsync()
    {
        await base.InitAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                               // No way to check nullability before awaiting
        WebWorker = GetWebWorker();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                               // Not to block the page
        WebWorker.ContinueWith(t => t.Result.InvokeAsync<IRoslynService, string>(nameof(IRoslynService.CompileAndRun), "class Program { public static void Main() {}}"));
#pragma warning restore CS4014
#pragma warning restore CS8619
    }

    private class ServiceProvider : IServiceProvider
    {
        private readonly object[] _services;
        public ServiceProvider(params object[] services) => _services = services;
        public object? GetService(Type serviceType) => _services.SingleOrDefault(s => s.GetType().IsAssignableTo(serviceType));
    }
}
