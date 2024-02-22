using System;
using Blazor.BrowserExtension;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder UseBrowserExtension(this WebAssemblyHostBuilder builder, Action<IBrowserExtensionEnvironment> setup = null)
        {
            builder.Services.AddBrowserExtensionServices();
            setup?.Invoke(IBrowserExtensionEnvironment.Instance);
            return builder;
        }
    }
}
