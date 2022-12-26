using System.ComponentModel;

namespace InterviewDotNet.ViewModels;

public abstract class ViewModel : IViewModel, INotifyPropertyChanged
{
    // Auto-injected
    public Action StateHasChanged { get; set; } = default!;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        StateHasChanged();
    }
}
