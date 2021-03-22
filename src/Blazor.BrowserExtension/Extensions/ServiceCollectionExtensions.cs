using Blazor.BrowserExtension;
using System;
using WebExtension.Net;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBrowserExtensionServices(this IServiceCollection services, Action<BrowserExtensionOption> optionSetup = null)
        {
            services.AddTransient<IWebExtensionAPI, WebExtensionAPI>();
            services.AddTransient<WebExtensionJSRuntime>();

            var option = new BrowserExtensionOption();
            optionSetup?.Invoke(option);
            services.AddSingleton(option);

            return services;
        }
    }
}
