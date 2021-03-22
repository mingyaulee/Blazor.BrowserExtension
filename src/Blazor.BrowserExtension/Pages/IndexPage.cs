using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Blazor.BrowserExtension.Pages
{
    public class IndexPage : BasePage
    {
        protected override void OnParametersSet()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("path", out var paths))
            {
                var path = paths.FirstOrDefault();
                if (!string.IsNullOrEmpty(path))
                {
                    Navigation.NavigateTo(path);
                }
            }
            base.OnParametersSet();
        }
    }
}
