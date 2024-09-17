using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebExtensions.Net.Manifest;
using WebExtensions.Net.Permissions;
using WebExtensions.Net.WebRequest;

namespace Blazor.BrowserExtension.Pages
{
    public partial class BackgroundPage : BasePage
    {
        private string prefixUri;

        [Inject]
        public IWebAssemblyHostEnvironment WebAssemblyHostEnvironment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            prefixUri = WebAssemblyHostEnvironment.BaseAddress;
            if (await WebExtensions.Permissions.Contains(new AnyPermissions()
            {
                Permissions = new Permission[]
                {
                    new OptionalPermission("webRequest"),
                    new OptionalPermission("webRequestBlocking")
                }
            }))
            {
                WebExtensions.WebRequest.OnBeforeRequest.AddListener(
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

        private BlockingResponse HandleBeforeRequestChange(OnBeforeRequestEventCallbackDetails details)
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

#if NET7_0_OR_GREATER
        private static bool IsStaticFile(string url)
        {
            return GetStaticFileRegex().IsMatch(url);
        }

        [GeneratedRegex(@"\.\w{2,5}(\?.+)?$", RegexOptions.IgnoreCase)]
        private static partial Regex GetStaticFileRegex();
#else
        private static bool IsStaticFile(string url)
        {
            return Regex.IsMatch(url, @"\.\w{2,5}(\?.+)?$");
        }
#endif
}
}
