namespace InterviewDotNet.ViewModels;

public class RootViewModel : ViewModel
{
    public RootViewModel()
    {
        Sample1 = new("""
using System;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Sample 1");
    }
}
""");
        Sample2 = new("""
using System;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Sample 2");
    }
}
""");
    }

    public FiddleViewModel Sample1 { get; }

    public FiddleViewModel Sample2 { get; }
}
