using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.BrowserExtension.IntegrationTest
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#Blazor_BrowserExtension_IntegrationTest_app");

            builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBrowserExtensionServices();

            await builder.Build().RunAsync();
        }
    }
}
