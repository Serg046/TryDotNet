namespace InterviewDotNet.ViewModels;

public class RootViewModel : ViewModel
{
    public RootViewModel(IViewModelFactory vmFactory)
    {
        Sample1 = vmFactory.Create<FiddleViewModel.Create>()(@"using System;

class Program
{
    public static void Main()
    {
        Console.WriteLine(""Sample 1"");
        var x = 23;
        x.
    }
}
");
        Sample2 = vmFactory.Create<FiddleViewModel.Create>()(@"
using System;

class Program
{
    public static void Main()
    {
        Console.WriteLine(""Sample 2"");
    }
}
");
    }

    public FiddleViewModel Sample1 { get; }

    public FiddleViewModel Sample2 { get; }
}
