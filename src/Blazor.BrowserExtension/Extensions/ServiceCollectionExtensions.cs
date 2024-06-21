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

#if NET7_0_OR_GREATER
            var browserExtensionEnvironment = GetBrowserExtensionEnvironment();
#else
            var browserExtensionEnvironment = GetBrowserExtensionEnvironment((IJSUnmarshalledRuntime)jsRuntime);
#endif
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

#if NET7_0_OR_GREATER
        private static BrowserExtensionEnvironment GetBrowserExtensionEnvironment()
#else
        private static BrowserExtensionEnvironment GetBrowserExtensionEnvironment(IJSUnmarshalledRuntime jsRuntime)
#endif
        {
#if NET7_0_OR_GREATER
            var browserExtensionEnvironmentValues = GetBrowserExtensionEnvironmentInterop().Split('|');
            var browserExtensionModeString = browserExtensionEnvironmentValues[0];
            var baseUrl = browserExtensionEnvironmentValues[1];
#else
            var browserExtensionModeString = jsRuntime.InvokeUnmarshalled<string>($"BlazorBrowserExtension.BrowserExtension._getBrowserExtensionModeLegacy");
            var baseUrl = "BaseUrl is not supported in .Net 6";
#endif
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

#if NET7_0_OR_GREATER

        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.BlazorBrowserExtension.BrowserExtension._getBrowserExtensionEnvironment")]
        private static partial string GetBrowserExtensionEnvironmentInterop();
#endif
    }
}
