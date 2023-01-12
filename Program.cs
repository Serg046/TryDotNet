using InterviewDotNet;
using InterviewDotNet.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<ParameterBasedRouter>("#app");
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
var defaultProvider = builder.Services.BuildServiceProvider();
builder.Services.AddContainer(
    new HttpClient { BaseAddress = baseAddress },
    defaultProvider.GetRequiredService<IJSRuntime>(),
    defaultProvider.GetRequiredService<NavigationManager>());
await builder.Build().RunAsync();
