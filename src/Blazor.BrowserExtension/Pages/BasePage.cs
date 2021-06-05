using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using WebExtension.Net;

namespace Blazor.BrowserExtension.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        public IWebExtensionApi WebExtension { get; set; }

        [Obsolete("This property is deprecated and will be removed in the next minor version release. Add @inject NavigationManager Navigation to continue using it.")]
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Obsolete("This property is deprecated and will be removed in the next minor version release. Add @inject IWebAssemblyHostEnvironment HostEnvironment to continue using it.")]
        [Inject]
        public IWebAssemblyHostEnvironment HostEnvironment { get; set; }

        [Inject]
        public ILogger<BasePage> Logger { get; set; }

        [Obsolete("This property is deprecated and will be removed in the next minor version release. Add @inject IJSRuntime JavaScript to continue using it.")]
        [Inject]
        public IJSRuntime JavaScript { get; set; }

        [Obsolete("This property is deprecated and will be removed in the next minor version release. Add @inject BrowserExtensionOption BrowserExtensionOption to continue using it.")]
        [Inject]
        public BrowserExtensionOption BrowserExtensionOption { get; set; }

        [Obsolete("This method is deprecated and will be removed in the next minor version release. Please open an issue in GitHub if you think it needs to be retained.")]
        protected string GetBrowserExtension()
        {
            return $"globalThis.BlazorBrowserExtension.{BrowserExtensionOption.GetSafeProjectNamespace()}";
        }

        [Obsolete("This method is deprecated and will be removed in the next minor version release. Use ES6 import instead.", UrlFormat = "https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/call-javascript-from-dotnet?view=aspnetcore-5.0#javascript-isolation-in-javascript-modules")]
        protected async Task ImportAsync(string script)
        {
            await JavaScript.InvokeVoidAsync($"{GetBrowserExtension()}.ImportAsync", script);
        }
    }
}
