using InterviewDotNet;
using InterviewDotNet.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
builder.RootComponents.Add<ParameterBasedRouter>("#app");
builder.Services.AddContainer(new HttpClient { BaseAddress = baseAddress });
await builder.Build().RunAsync();
