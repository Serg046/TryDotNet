using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Web;

namespace InterviewDotNet.Components;

public class ParameterBasedRouter : IComponent, IHandleAfterRender, IDisposable
{
    private RenderHandle _renderHandle;
    private bool _navigationInterceptionEnabled;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private INavigationInterception NavigationInterception { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    
    Task IHandleAfterRender.OnAfterRenderAsync()
    {
        if (!_navigationInterceptionEnabled)
        {
            _navigationInterceptionEnabled = true;
            return NavigationInterception.EnableNavigationInterceptionAsync();
        }

        return Task.CompletedTask;
    }

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Render(e.Location);
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        Render(NavigationManager.Uri);
        return Task.CompletedTask;
    }

    private void Render(string location) => Render(new Uri(location));
    private void Render(Uri location)
    {
        JSRuntime.InvokeVoidAsync("selectLinkBasedOnLocation");
        _renderHandle.Render(c =>
        {
            c.OpenComponent(0, GetPageType(location));
            c.CloseComponent();
        });
    }

    private Type GetPageType(Uri location)
    {
        var parameters = HttpUtility.ParseQueryString(location.Query);
        return parameters["p"] switch
        {
            "sample2" => typeof(Sample2),
            _ => typeof(Sample1)
        };
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
