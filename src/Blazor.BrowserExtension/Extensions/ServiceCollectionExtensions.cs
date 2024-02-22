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
#if NET7_0_OR_GREATER
            var browserExtensionEnvironment = new BrowserExtensionEnvironment(GetBrowserExtensionMode());
#else
            if (services.FirstOrDefault(service => service.ServiceType == typeof(IJSRuntime))?.ImplementationInstance is not IJSUnmarshalledRuntime jsRuntime)
            {
                throw new NotSupportedException("An instance of IJSUnmarshalledRuntime must be registered by Blazor.");
            }

            var browserExtensionEnvironment = new BrowserExtensionEnvironment(GetBrowserExtensionMode(jsRuntime));
#endif
            IBrowserExtensionEnvironment.Instance = browserExtensionEnvironment;
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

#if NET7_0_OR_GREATER
        private static BrowserExtensionMode GetBrowserExtensionMode()
#else
        private static BrowserExtensionMode GetBrowserExtensionMode(IJSUnmarshalledRuntime jsRuntime)
#endif
        {
#if NET7_0_OR_GREATER
            var browserExtensionModeString = GetBrowserExtensionModeInterop();
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
        private static partial string GetBrowserExtensionModeInterop();
#endif
    }
}
