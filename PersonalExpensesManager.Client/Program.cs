using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalExpensesManager.Client;
using PersonalExpensesManager.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5012/") });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthTokenStore>();
builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AppAuthenticationStateProvider>());
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<FinanceApiClient>();

await builder.Build().RunAsync();
