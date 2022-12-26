using InterviewDotNet.Components;
using InterviewDotNet.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

const string baseTag = "/";
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), baseTag);
builder.RootComponents.Add<ParameterBasedRouter>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
builder.Services.AddSingleton<RootViewModel>();
await builder.Build().RunAsync();
