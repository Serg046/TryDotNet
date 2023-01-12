using BlazorMonaco;
using InterviewDotNet.Models;
using InterviewDotNet.Services;
using Microsoft.JSInterop;
using SpawnDev.BlazorJS.WebWorkers;
using System.Diagnostics.CodeAnalysis;

namespace InterviewDotNet.ViewModels;

public class FiddleViewModel : ViewModel
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<Task<WebWorker>> _webWorker;
    private DotNetObjectReference<FiddleViewModel>? _fiddleViewModelRef;

    public delegate FiddleViewModel Create(string sample);
    public FiddleViewModel(IJSRuntime jsRuntime, IViewModelFactory vmFactory, WebWorkerService webWorkerService, string sample)
    {
        Sample = Code = sample;
        _jsRuntime = jsRuntime;
        _webWorker = new Lazy<Task<WebWorker>>(async () => await webWorkerService.GetWebWorker() ?? throw new PlatformNotSupportedException("Cannot create a worker"));
        SessionViewModel = vmFactory.Create<SessionViewModel.Create>()();
    }

    public SessionViewModel SessionViewModel { get; }

    [AllowNull]
    public MonacoEditor Editor { get; set; }

    public string Sample { get; }

    public string Code { get; set; }

    private string _output = "";
    public string Output
    {
        get => _output;
        set
        {
            _output = value;
            RaisePropertyChanged(nameof(Output));
        }
    }

    public StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor _) => new() { Language = "csharp", Value = Code };

    public async Task OnAfterRenderAsync()
    {
        if (_fiddleViewModelRef is null)
        {
            _fiddleViewModelRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeAsync<string>("registerMonacoProviders", _fiddleViewModelRef);
            await RunInWorker<IRoslynService, string>(nameof(IRoslynService.CompileAndRun), Sample);
        }
    }

    private async Task<TReturn> RunInWorker<TService, TReturn>(string methodName, params object?[]? args)
    {
        var webWorker = await _webWorker.Value;
        return await webWorker.InvokeAsync<TService, TReturn>(methodName, args);
    }

    public async void Run()
    {
        Output = await RunInWorker<IRoslynService, string>(nameof(IRoslynService.CompileAndRun), await Editor.GetValue());
        StateHasChanged();
    }

    public async void Reset()
    {
        Code = Sample;
        await Editor.SetValue(Code);
        Output = string.Empty;
        StateHasChanged();
    }

    [JSInvokable]
    public Task<IReadOnlyList<Completion>> GetCompletions(string code, CompletionRequest request)
    {
        return RunInWorker<IRoslynService, IReadOnlyList<Completion>>(nameof(IRoslynService.GetCompletions), code, request);
    }

    public void ToggleSession()
    {
        SessionViewModel.IsShown = !SessionViewModel.IsShown;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        Code = await Editor.GetValue();
    }
}
