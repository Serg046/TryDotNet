using InterviewDotNet.Services;
using InterviewDotNet.ViewModels;
using Pure.DI;

namespace InterviewDotNet;

internal static partial class Container
{
    internal static void Setup() => DI.Setup()
        .Bind<IMonacoService>().To<MonacoService>()
        .Bind<IViewModelFactory>().To<ViewModelFactory>()
        .Bind<FiddleViewModel.Create>().To(ctx => new FiddleViewModel.Create(code => new FiddleViewModel(code)))
        .Bind<RootViewModel>().To<RootViewModel>();
}
