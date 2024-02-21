using System.Threading.Tasks;
using Docs.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Docs
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--after-publish")
            {
                AfterPublish.Run(args[1]);
                return;
            }

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddTransient(sp => (IJSInProcessRuntime)sp.GetRequiredService<IJSRuntime>());
            builder.Services.AddSingleton<DocumentRouteEvent>();
            await builder.Build().RunAsync();
        }
    }
}
