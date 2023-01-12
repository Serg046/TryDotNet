using InterviewDotNet.Services;
using InterviewDotNet.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pure.DI;
using SpawnDev.BlazorJS.WebWorkers;

namespace InterviewDotNet;

internal static partial class Container
{
    internal static void Setup() => DI.Setup()
        .Arg<HttpClient>()
        .Arg<IJSRuntime>()
        .Arg<NavigationManager>()
        .Bind<WebWorkerService>().To(ctx => new WebWorkerService(ctx.Resolve<NavigationManager>(), ctx.Resolve<HttpClient>(), new ServiceProvider(ctx.Resolve<IRoslynService>())))
        .Bind<IRoslynService>().To<RoslynService>()
        .Bind<IViewModelFactory>().To<ViewModelFactory>()
        .Bind<FiddleViewModel.Create>().To(ctx => new FiddleViewModel.Create(code => new FiddleViewModel(ctx.Resolve<IJSRuntime>(), ctx.Resolve<IViewModelFactory>(), ctx.Resolve<WebWorkerService>(), code)))
        .Bind<SessionViewModel.Create>().To(ctx => new SessionViewModel.Create(() => new SessionViewModel()))
        .Bind<RootViewModel>().As(Lifetime.Singleton).To<RootViewModel>();

    private class ServiceProvider : IServiceProvider
    {
        private readonly IRoslynService _roslynService;

        public ServiceProvider(IRoslynService roslynService)
        {
            _roslynService = roslynService;
        }

        public object? GetService(Type serviceType)
        {
            return _roslynService;
        }
    }
}
