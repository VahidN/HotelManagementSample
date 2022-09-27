using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorWasm.Client;
using BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after"); //TODO: don't start addresses with `/` every where

// dotnet add package Microsoft.Extensions.Http
builder.Services.AddHttpClient(
                               "ServerAPI",
                               client =>
                               {
                                   client.BaseAddress =
                                       new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl"));
                                   client.DefaultRequestHeaders.Add("User-Agent", "BlazorWasm.Client 1.0");
                               }
                              )
       .AddHttpMessageHandler<ClientHttpInterceptorService>();
builder.Services.AddScoped<ClientHttpInterceptorService>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddScoped<IClientHotelRoomService, ClientHotelRoomService>();
builder.Services.AddScoped<IClientHotelAmenityService, ClientHotelAmenityService>();
builder.Services.AddScoped<IClientRoomOrderDetailsService, ClientRoomOrderDetailsService>();
builder.Services.AddScoped<IClientAuthenticationService, ClientAuthenticationService>();
builder.Services.AddScoped<IClientProtectedApiService, ClientProtectedApiService>();

builder.Services.AddAuthorizationCore(options => options.AddAppPolicies());
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>();

builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();