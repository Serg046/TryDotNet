namespace InterviewDotNet.ViewModels;

public class SessionViewModel : ViewModel
{
    public delegate SessionViewModel Create();

    private bool _isShown;
    public bool IsShown
    {
        get => _isShown;
        set
        {
            _isShown = value;
            RaisePropertyChanged(nameof(IsShown));
        }
    }
}
