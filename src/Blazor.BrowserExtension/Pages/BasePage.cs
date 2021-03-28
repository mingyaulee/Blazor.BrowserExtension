using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using WebExtension.Net;

namespace Blazor.BrowserExtension.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        public IWebExtensionApi WebExtension { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment HostEnvironment { get; set; }

        [Inject]
        public ILogger<BasePage> Logger { get; set; }

        [Inject]
        public IJSRuntime JavaScript { get; set; }

        [Inject]
        public BrowserExtensionOption BrowserExtensionOption { get; set; }

        protected string GetBrowserExtension()
        {
            return $"globalThis.BlazorBrowserExtension.{BrowserExtensionOption.GetSafeProjectNamespace()}";
        }

        protected async Task ImportAsync(string script)
        {
            await JavaScript.InvokeVoidAsync($"{GetBrowserExtension()}.ImportAsync", script);
        }
    }
}
