namespace InterviewDotNet.ViewModels;

public interface IViewModel
{
    Action StateHasChanged { get; set; }
}
