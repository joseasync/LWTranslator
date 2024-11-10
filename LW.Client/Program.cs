using LW.Client;
using LW.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress =
    new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:7205")
});

builder.Services.AddHttpClient("lwapi", opt => opt.BaseAddress =
new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:7160"))
    .AddHttpMessageHandler<CustomHttpHandler>();

await builder.Build().RunAsync();
