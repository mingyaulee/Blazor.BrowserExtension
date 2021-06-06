using Blazor.BrowserExtension;
using Microsoft.JSInterop;
using System;
using System.Linq;
using WebExtensions.Net;
using WebExtensions.Net.Mock;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBrowserExtensionServices(this IServiceCollection services, Action<BrowserExtensionOption> optionSetup = null)
        {
            var option = new BrowserExtensionOption();
            optionSetup?.Invoke(option);
            services.AddSingleton(option);

            if (services.FirstOrDefault(service => service.ServiceType == typeof(IJSRuntime))?.ImplementationInstance is not IJSUnmarshalledRuntime jsRuntime)
            {
                throw new NotSupportedException("An instance of IJSUnmarshalledRuntime must be registered by Blazor.");
            }

            var browserExtensionEnvironment = InitializeBrowserExtensionEnvironment(jsRuntime, option);
            services.AddSingleton<IBrowserExtensionEnvironment>(browserExtensionEnvironment);
            services.AddTransient<IWebExtensionsApi, WebExtensionsApi>();

            if (browserExtensionEnvironment.Mode == BrowserExtensionMode.Debug)
            {
                services.AddSingleton<IWebExtensionsJSRuntime, MockWebExtensionsJSRuntime>();
            }
            else
            {
                services.AddSingleton<IWebExtensionsJSRuntime, WebExtensionsJSRuntime>();
            }

            return services;
        }

        private static BrowserExtensionEnvironment InitializeBrowserExtensionEnvironment(IJSUnmarshalledRuntime jsRuntime, BrowserExtensionOption option)
        {
            return new(GetBrowserExtensionMode(jsRuntime, option));
        }

        private static BrowserExtensionMode GetBrowserExtensionMode(IJSUnmarshalledRuntime jsRuntime, BrowserExtensionOption option)
        {
            var browserExtensionModeString = jsRuntime.InvokeUnmarshalled<string>($"BlazorBrowserExtension.{option.GetSafeProjectNamespace()}._getBrowserExtensionMode");
            if (Enum.TryParse<BrowserExtensionMode>(browserExtensionModeString, out var result))
            {
                return result;
            }
            else
            {
                return BrowserExtensionMode.Standard;
            }
        }
    }
}
