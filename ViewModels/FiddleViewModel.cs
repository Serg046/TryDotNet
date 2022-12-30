namespace InterviewDotNet.ViewModels;

public class FiddleViewModel : ViewModel
{
    public delegate FiddleViewModel Create(string sample);
    public FiddleViewModel(string sample)
    {
        Sample = Code = sample;
    }

    public string Sample { get; }

    public string Code { get; set; }

    private string _output = "";
    public string Output
    {
        get => _output; set
        {
            _output = value;
            RaisePropertyChanged(nameof(Output));
        }
    }
}
