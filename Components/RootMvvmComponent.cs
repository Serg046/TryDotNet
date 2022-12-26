using InterviewDotNet.ViewModels;
using Microsoft.AspNetCore.Components;

namespace InterviewDotNet.Components;

public class RootMvvmComponent<T> : ComponentBase where T : IViewModel
{
    // Auto-injected
    private T _dataContext = default!;
    [Inject] // Auto-injected
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
