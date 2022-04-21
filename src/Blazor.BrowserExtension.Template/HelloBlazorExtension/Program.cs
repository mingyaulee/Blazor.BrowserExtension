using System;
using System.Net.Http;
using System.Threading.Tasks;
#if (!IsNet5)
using Microsoft.AspNetCore.Components.Web;
#endif
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
#if (!IsNet5)
            builder.RootComponents.Add<HeadOutlet>("head::after");
#endif

#if (IsNet5)
            // workaround to use JavaScript fetch to bypass url validation
            // see: https://github.com/dotnet/runtime/issues/52836
            builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#else
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#endif

            builder.Services.AddBrowserExtensionServices();
            await builder.Build().RunAsync();
        }
    }
}
