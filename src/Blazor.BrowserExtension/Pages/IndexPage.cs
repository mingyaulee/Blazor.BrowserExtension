using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.BrowserExtension.Pages
{
    public class IndexPage : BasePage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override void OnParametersSet()
        {
            if (TryGetPath(NavigationManager.Uri, out var path))
            {
                NavigationManager.NavigateTo(path);
            }
            base.OnParametersSet();
        }

        private static bool TryGetPath(string url, out string path)
        {
            path = null;
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            var queryStartIndex = url.IndexOf('?');
            if (queryStartIndex == -1)
            {
                return false;
            }

            var query = url[(queryStartIndex + 1)..];
            foreach (var queryParam in query.Split('&'))
            {
                var keyValue = queryParam.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim().Equals("path", StringComparison.OrdinalIgnoreCase))
                {
                    path = Uri.UnescapeDataString(keyValue[1].Trim());
                    return !string.IsNullOrEmpty(path);
                }
            }

            return false;
        }
    }
}
