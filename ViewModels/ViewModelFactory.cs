using System.Reflection;

namespace InterviewDotNet.ViewModels;

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : Delegate
    {
#if (DEBUG)
        var type = typeof(T);            
        if (type.GetMethod("Invoke") is not MethodInfo method || !method.ReturnType.IsAssignableTo(typeof(IViewModel)))
        {
            throw new NotSupportedException("ViewModelFactory is for view models only");
        }
#endif
        return _serviceProvider.GetRequiredService<T>();
    }
}
