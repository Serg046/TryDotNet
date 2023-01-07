using InterviewDotNet;
using InterviewDotNet.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
builder.RootComponents.Add<ParameterBasedRouter>("#app");
var defaultProvider = builder.Services.BuildServiceProvider();
builder.Services.AddContainer(
    new HttpClient { BaseAddress = baseAddress },
    defaultProvider.GetRequiredService<IJSRuntime>());
await builder.Build().RunAsync();
