using InterviewDotNet.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWorker.Core;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
builder.RootComponents.Add<ParameterBasedRouter>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
builder.Services.AddWorkerFactory();
await builder.Build().RunAsync();
