using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HelloBlazorExtension
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // please notice: there is a issue about base address in browser extension mode.
            // see: https://github.com/dotnet/runtime/issues/52836
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBrowserExtensionServices(options =>
            {
                options.ProjectNamespace = typeof(Program).Namespace;
            });
            await builder.Build().RunAsync();
        }
    }
}
