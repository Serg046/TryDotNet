namespace InterviewDotNet.ViewModels;

public interface IViewModelFactory
{
    T Create<T>() where T : Delegate;
}
