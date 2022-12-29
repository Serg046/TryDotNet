﻿namespace InterviewDotNet.ViewModels;

public class FiddleViewModel : ViewModel
{
    public delegate FiddleViewModel Create(string initialCode);
    public FiddleViewModel(string initialCode)
    {
        InitialCode = initialCode;
    }

    public string InitialCode { get; }

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