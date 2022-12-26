using InterviewDotNet.ViewModels;
using Microsoft.AspNetCore.Components;

namespace InterviewDotNet.Components;

public class MvvmComponent<T> : ComponentBase where T : IViewModel
{
    // Auto-injected
    private T _dataContext = default!;
    [Parameter, EditorRequired]
#pragma warning disable BL0007 // StateHasChanged is not called here but passed through
    public T DataContext
#pragma warning restore BL0007 // Component parameters should be auto properties
    {
        get => _dataContext;
        set
        {
            _dataContext = value;
            _dataContext.StateHasChanged = StateHasChanged;
        }
    }
}
