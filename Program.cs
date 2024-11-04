using IndexedDB.Blazor;
using JusGiveawayWebApp;
using JusGiveawayWebApp.Helpers;
using JusGiveawayWebApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new FirebaseService(new HttpClient(), "https://jusgiveaway-default-rtdb.europe-west1.firebasedatabase.app"));
builder.Services.AddScoped<CommonFunctions>();
builder.Services.AddScoped<AdminFunctions>();
builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddSingleton<UserInfoService>();

await builder.Build().RunAsync();
