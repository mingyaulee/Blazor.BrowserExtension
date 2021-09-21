using Blazor.BrowserExtension;
using Microsoft.JSInterop;
using System;
using System.Linq;

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
            var browserExtensionModeString = jsRuntime.InvokeUnmarshalled<string>($"BlazorBrowserExtension.BrowserExtension._getBrowserExtensionMode");
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
