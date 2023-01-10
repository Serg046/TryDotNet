using InterviewDotNet.Services;
using InterviewDotNet.ViewModels;
using Microsoft.JSInterop;
using Pure.DI;

namespace InterviewDotNet;

internal static partial class Container
{
    internal static void Setup() => DI.Setup()
        .Arg<HttpClient>()
        .Arg<IJSRuntime>()
        .Bind<IRoslynService>().To<RoslynService>()
        .Bind<IViewModelFactory>().To<ViewModelFactory>()
        .Bind<FiddleViewModel.Create>().To(ctx => new FiddleViewModel.Create(code => new FiddleViewModel(ctx.Resolve<IRoslynService>(), ctx.Resolve<IJSRuntime>(), ctx.Resolve<IViewModelFactory>(), code)))
        .Bind<SessionViewModel.Create>().To(ctx => new SessionViewModel.Create(() => new SessionViewModel()))
        .Bind<RootViewModel>().As(Lifetime.Singleton).To<RootViewModel>();
}
