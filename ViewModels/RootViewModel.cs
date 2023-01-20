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
using System.Linq.Expressions;
using System.Collections.Generic;

class Program
{
    public static void Main()
    {
        var service = new EmployeeService();
        var employees = new EntityCollection<Employee>
        {
            new Employee("John", 45),
            new Employee("Paul", 55)
        };
        service.ProcessEmployees(() => employees);
    }
}

record Employee(string FullName, int Age);

class EmployeeService
{
    public void ProcessEmployees(Func<IQueryable<Employee>> employeeEntityFrameworkRepository)
    {
        var employees1 = employeeEntityFrameworkRepository()
            .Where(employee => IsEmployeeUnder(employee, 50))
            .OrderBy(employee => employee.FullName)
            .Take(10)
            .ToList();

        var employees2 = employeeEntityFrameworkRepository()
            .AsEnumerable()
            .Where(employee => employee.Age < 50)
            .OrderBy(employee => employee.FullName)
            .Take(10)
            .ToList();

        Console.WriteLine(string.Join(", ", employees1.Concat(employees2).Select(e => e.FullName)));
    }

    private static bool IsEmployeeUnder(Employee employee, int age) => employee.Age < age;
}

class EntityCollection<T> : List<T>, IQueryable<T>
{
    public Type ElementType => typeof(T);
    public Expression Expression => Expression.Empty();
    public System.Linq.IQueryProvider Provider => null;
}
""");
        Sample6 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    public static void Main()
    {
        var list1 = Enumerable.Range(1, 100).ToList();
        var list2 = new List<int>();

        Parallel.ForEach(list1, element => {
            list2.Add(element + 2); // Does this cause any problems?
        });

        Console.WriteLine("Done");
    }
}
""");
        Sample7 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Threading.Tasks;

class Program
{
    public static void Main()
    {
        var actionTask = StartLongAction();
        Console.WriteLine("Do something useful");
        actionTask.Wait();
        Console.WriteLine("Done");
    }

    private static async Task StartLongAction()
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
	}
}
""");
        Sample8 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;

class A
{
    public virtual void PrintInfo(object x)
    {
        Console.WriteLine("A/object");
    }
}

class B : A
{
    public override void PrintInfo(object x)
    {
        Console.WriteLine("B/object");
    }

    public void PrintInfo(string x)
    {
        Console.WriteLine("B/string");
    }
}

class Program
{
    public static void Main()
    {
        var argument = GetArgument();
        A a = new B();

        // What's the output here?
        a.PrintInfo(argument);
    }

    private static string GetArgument() => "some string";
}
""");
        Sample9 = vmFactory.Create<FiddleViewModel.Create>()("""
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public static void Main()
    {
        Console.WriteLine("With ToArray() call");
        foreach (var item in GetNumbers(2).ToArray())
        {
            // What's the output?
            Console.Write(item);
        }
        Console.WriteLine();

        Console.WriteLine("Without ToArray() call");
        foreach (var item in GetNumbers(2))
        {
            // What's the output?
            Console.Write(item);
        }
    }

    private static IEnumerable<int> GetNumbers(int count)
    {
        for (int index = 0; index < count; index++)
        {
            Console.Write(index);
            yield return index;
        }
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
    public FiddleViewModel Sample7 { get; }
    public FiddleViewModel Sample8 { get; }
    public FiddleViewModel Sample9 { get; }
}
