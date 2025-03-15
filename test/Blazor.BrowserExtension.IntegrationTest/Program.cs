using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.BrowserExtension;
using Blazor.BrowserExtension.IntegrationTest;
using Blazor.BrowserExtension.IntegrationTest.Pages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.UseBrowserExtension(browserExtension =>
{
    if (browserExtension.Mode == BrowserExtensionMode.ContentScript)
    {
        builder.RootComponents.Add<ContentScript>("#Blazor_BrowserExtension_IntegrationTest_app");
    }
    else if (browserExtension.Mode == BrowserExtensionMode.Background)
    {
        builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();
    }
    else
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    }
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
