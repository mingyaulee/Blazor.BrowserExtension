using System;
using Blazor.BrowserExtension;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
#pragma warning restore IDE0130 // Namespace does not match folder structure
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
