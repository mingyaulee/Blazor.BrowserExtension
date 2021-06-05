using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebExtension.Net.Manifest;
using WebExtension.Net.Permissions;
using WebExtension.Net.WebRequest;

namespace Blazor.BrowserExtension.Pages
{
    public class BackgroundPage : BasePage
    {
        private string prefixUri;

        [Inject]
        public IWebAssemblyHostEnvironment WebAssemblyHostEnvironment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            prefixUri = WebAssemblyHostEnvironment.BaseAddress;
            if (await WebExtension.Permissions.Contains(new AnyPermissions()
            {
                Permissions = new Permission[]
                {
                    new OptionalPermission("webRequest"),
                    new OptionalPermission("webRequestBlocking")
                }
            }))
            {
                await WebExtension.WebRequest.OnBeforeRequest.AddListener(
                    HandleBeforeRequestChange,
                    new RequestFilter()
                    {
                        Urls = new[] { prefixUri + "*" }
                    },
                    new[]
                    {
                        OnBeforeRequestOptions.Blocking
                    });
            }
        }

        private BlockingResponse HandleBeforeRequestChange(OnBeforeRequestEventAddListenerCallbackDetails details)
        {
            if (details.Url.StartsWith(prefixUri) && !IsStaticFile(details.Url))
            {
                var path = details.Url[prefixUri.Length..];
                var newUrl = prefixUri + "index.html";
                if (!string.IsNullOrEmpty(path))
                {
                    newUrl += "?path=" + Uri.EscapeDataString(path);
                }
                return new BlockingResponse()
                {
                    RedirectUrl = newUrl
                };
            }
            return null;
        }

        private static bool IsStaticFile(string url)
        {
            return Regex.IsMatch(url, @"\.\w{2,5}(\?.+)?$");
        }
    }
}
