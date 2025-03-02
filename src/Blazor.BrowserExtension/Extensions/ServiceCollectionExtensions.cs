using Blazor.BrowserExtension;
using Microsoft.JSInterop;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBrowserExtensionServices(this IServiceCollection services)
        {
            var iJsRuntimeService = services.FirstOrDefault(service => service.ServiceType == typeof(IJSRuntime));
            if (iJsRuntimeService?.ImplementationInstance is not IJSRuntime jsRuntime)
            {
                throw new NotSupportedException("An instance of IJSRuntime must be registered by Blazor.");
            }

            var browserExtensionEnvironment = GetBrowserExtensionEnvironment();
            IBrowserExtensionEnvironment.Instance = browserExtensionEnvironment;
            services.AddSingleton<IBrowserExtensionEnvironment>(browserExtensionEnvironment);

            services.Remove(iJsRuntimeService);
            services.AddSingleton<IJSRuntime>(new ProxyJsRuntime(jsRuntime, browserExtensionEnvironment));

            if (browserExtensionEnvironment.Mode == BrowserExtensionMode.Debug)
            {
                services.AddMockWebExtensions();
            }
            else
            {
                services.AddWebExtensions();
            }

            return services;
        }

        private static BrowserExtensionEnvironment GetBrowserExtensionEnvironment()
        {
            var browserExtensionEnvironmentValues = GetBrowserExtensionEnvironmentInterop().Split('|');
            var browserExtensionModeString = browserExtensionEnvironmentValues[0];
            var baseUrl = browserExtensionEnvironmentValues[1];
            if (!Enum.TryParse<BrowserExtensionMode>(browserExtensionModeString, out var mode))
            {
                mode = BrowserExtensionMode.Standard;
            }

            return new BrowserExtensionEnvironment()
            {
                Mode = mode,
                BaseUrl = baseUrl
            };
        }


        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.BlazorBrowserExtension.BrowserExtension._getBrowserExtensionEnvironment")]
        private static partial string GetBrowserExtensionEnvironmentInterop();
    }
}
