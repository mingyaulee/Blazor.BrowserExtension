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
            if (services.FirstOrDefault(service => service.ServiceType == typeof(IJSRuntime))?.ImplementationInstance is not IJSUnmarshalledRuntime jsRuntime)
            {
                throw new NotSupportedException("An instance of IJSUnmarshalledRuntime must be registered by Blazor.");
            }

            var browserExtensionEnvironment = new BrowserExtensionEnvironment(GetBrowserExtensionMode(jsRuntime));
            services.AddSingleton<IBrowserExtensionEnvironment>(browserExtensionEnvironment);

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

        private static BrowserExtensionMode GetBrowserExtensionMode(IJSUnmarshalledRuntime jsRuntime)
        {
#if NET7_0_OR_GREATER
            var browserExtensionModeString = GetBrowserExtensionMode();
#else
            var browserExtensionModeString = jsRuntime.InvokeUnmarshalled<string>($"BlazorBrowserExtension.BrowserExtension._getBrowserExtensionModeLegacy");
#endif
            if (Enum.TryParse<BrowserExtensionMode>(browserExtensionModeString, out var result))
            {
                return result;
            }
            else
            {
                return BrowserExtensionMode.Standard;
            }
        }

#if NET7_0_OR_GREATER

        [System.Runtime.InteropServices.JavaScript.JSImport("globalThis.BlazorBrowserExtension.BrowserExtension._getBrowserExtensionMode")]
        private static partial string GetBrowserExtensionMode();
#endif
    }
}
