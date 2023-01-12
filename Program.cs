using InterviewDotNet;
using InterviewDotNet.Components;
using InterviewDotNet.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebWorkers;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
if (JS.IsWindow)
{
    builder.RootComponents.Add<ParameterBasedRouter>("#app");
}
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
var defaultProvider = builder.Services.BuildServiceProvider();
builder.Services.AddContainer(
    new HttpClient { BaseAddress = baseAddress },
    defaultProvider.GetRequiredService<IJSRuntime>(),
    defaultProvider.GetRequiredService<NavigationManager>());
var host = builder.Build();
await host.Services.GetRequiredService<WebWorkerService>().InitAsync();
await host.RunAsync();
