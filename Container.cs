using InterviewDotNet.Services;
using InterviewDotNet.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pure.DI;

namespace InterviewDotNet;

internal static partial class Container
{
    internal static void Setup() => DI.Setup()
        .Arg<HttpClient>()
        .Arg<IJSRuntime>()
        .Arg<NavigationManager>()
        .Bind<IRoslynService>().To<RoslynService>()
        .Bind<IViewModelFactory>().To<ViewModelFactory>()
        .Bind<FiddleViewModel.Create>().To(ctx => new FiddleViewModel.Create(code => new FiddleViewModel(ctx.Resolve<IJSRuntime>(), ctx.Resolve<IViewModelFactory>(), ctx.Resolve<IRoslynService>(), code)))
        .Bind<SessionViewModel.Create>().To(ctx => new SessionViewModel.Create(() => new SessionViewModel()))
        .Bind<RootViewModel>().As(Lifetime.Singleton).To<RootViewModel>(); // Shouldn't be reloaded via some sample click
}
