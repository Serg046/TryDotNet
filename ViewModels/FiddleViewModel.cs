using BlazorMonaco;
using InterviewDotNet.Services;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace InterviewDotNet.ViewModels;

public class FiddleViewModel : ViewModel
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IRoslynService _roslynService;
    private DotNetObjectReference<IRoslynService>? _roslynServiceRef;

    public delegate FiddleViewModel Create(string sample);
    public FiddleViewModel(IJSRuntime jsRuntime, IViewModelFactory vmFactory, IRoslynService roslynService, string sample)
    {
        Sample = Code = sample;
        _jsRuntime = jsRuntime;
        _roslynService = roslynService;
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

    public StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor _) => new() { Language = "csharp", Value = Code, AutomaticLayout = true };

    public async Task OnAfterRenderAsync()
    {
        if (_roslynServiceRef is null)
        {
            _roslynServiceRef = DotNetObjectReference.Create(_roslynService);
            await _jsRuntime.InvokeAsync<string>("registerMonacoProviders", _roslynServiceRef);
        }
    }

    public async void Run()
    {
        Output = await _roslynService.CompileAndRun(await Editor.GetValue());
        StateHasChanged();
    }

    public async void Reset()
    {
        Code = Sample;
        await Editor.SetValue(Code);
        Output = string.Empty;
        StateHasChanged();
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
