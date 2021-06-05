using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Blazor.BrowserExtension.Pages
{
    public class IndexPage : BasePage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override void OnParametersSet()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("path", out var paths))
            {
                var path = paths.FirstOrDefault();
                if (!string.IsNullOrEmpty(path))
                {
                    NavigationManager.NavigateTo(path);
                }
            }
            base.OnParametersSet();
        }
    }
}
