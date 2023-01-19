namespace InterviewDotNet.ViewModels;

public class RootViewModel : ViewModel
{
    public RootViewModel(IViewModelFactory vmFactory)
    {
        Sample1 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Collections.Generic;

class Program
{
    public static void Main()
    {
		// What happens here?
        var circle = new Circle();
    }
}

public abstract class Shape
{
	public Shape()
	{
    	Draw();
	}

	public abstract void Draw();
}

public class Circle : Shape
{
	private List<Point> points;

	public Circle()
	{
    	points = new List<Point>() { new Point(1, 1), new Point(2, 2)};
	}

	public override void Draw()
	{
    	foreach (var point in points)
    	{
            //Dosomething();
    	}
	}
}

public record Point(int X, int Y);
""");
        Sample2 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.IO;

class Program
{
    public static void Main()
    {
        using (var writer = new StreamWriter("something.txt"))
        {
            char[] message = "Data".ToCharArray();
            // Throws an exception
            // What happens with the writer and with the exception?
            writer.Write(message, 0, 5);
        }
        Console.WriteLine("Done");
    }
}
""");
        Sample3 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;

public class TestClass
{
    public string Id { get; set; }
}

class Program
{
    public static void Main()
    {
        var test1 = new TestClass() { Id = "1" };
        Mutate(test1);
        var test2 = new TestClass() { Id = "2" };
        // What is printed here?
        Console.WriteLine(test1 == test2);
        Mutate2(test1);
        // What is printed here?
        Console.WriteLine(test1.Id);
        int test3 = 1;
        Mutate(test3);
        int test4 = 2;
        // What is printed here?
        Console.WriteLine(test3 == test4);
    }

    public static void Mutate(TestClass test)
    {
        test.Id = "2";
    }

    public static void Mutate(int test)
    {
        test = 2;
    }

    public static void Mutate2(TestClass test)
    {
        test = new TestClass() { Id = "3" };
    }
}
""");
        Sample4 = vmFactory.Create<FiddleViewModel.Create>()("""
using System.Collections.Generic;

public class TestClass
{
    public string Id { get; set; }
}

class Program
{
    public static void Main()
    {
        var testClass = new TestClass() { Id = "a" };

        // How to avoid this?
        testClass.Id = "c";

        var list = GetList();

        // How to avoid this?
        list.Add(testClass);
    }

    public static List<TestClass> GetList()
    {
        return new List<TestClass>
        {
            new TestClass() { Id = "a" },
            new TestClass() { Id = "b" }
        };
    }
}
""");
        Sample5 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Linq;

record Employee(string Fullname, int Age);

class EmployeeService
{
    public void ProcessEmployees(Func<IQueryable<Employee>> employeeEntityFrameworkRepository)
    {
        var employees1 = employeeEntityFrameworkRepository()
            .Where(employee => IsEmployeeUnder(employee, 50))
            .OrderBy(employee => employee.Fullname)
            .Take(10)
            .ToList();

        var employees2 = employeeEntityFrameworkRepository()
            .AsEnumerable()
            .Where(employee => employee.Age < 50)
            .OrderBy(employee => employee.Fullname)
            .Take(10)
            .ToList();
    }

    private static bool IsEmployeeUnder(Employee employee, int age) => employee.Age < age;
}
""");
        Sample6 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public record DataDto(string Data);

public interface IPagedGateway
{
    // Page starts from 1
    // DataDto properties are not relevant here
    Task<PageResult<DataDto>> GetPage(int pageNumber);
}

public class PageResult<T>
{
    // Records of this page
    public IEnumerable<T> PageRecords { get; init; }

    //Total pages available in the gateway
    public int TotalPages { get; init; }
}

class Program
{
    public static async Task Main()
    {
        var data = await GetAllData(new PagedGateway());
        Console.WriteLine(data.Count());
    }

    // Write some code that fetches all the pages and returns all the records available as an IEnumerable<DataDto>
    // In order to fetch the pages use as much parallelism as possible
    public static async Task<IEnumerable<DataDto>> GetAllData(IPagedGateway gateway)
    {
    }
}

public class PagedGateway : IPagedGateway
{
    public async Task<PageResult<DataDto>> GetPage(int pageNumber)
    {
        await Task.Delay(1);
        return pageNumber switch
        {
            1 => new PageResult<DataDto> { PageRecords = new List<DataDto> { new DataDto("data1") }, TotalPages = 2 },
            2 => new PageResult<DataDto> { PageRecords = new List<DataDto> { new DataDto("data2") }, TotalPages = 2 },
            _ => throw new IndexOutOfRangeException(),
        };
    }
}
""");
    }

    public FiddleViewModel Sample1 { get; }
    public FiddleViewModel Sample2 { get; }
    public FiddleViewModel Sample3 { get; }
    public FiddleViewModel Sample4 { get; }
    public FiddleViewModel Sample5 { get; }
    public FiddleViewModel Sample6 { get; }
}
