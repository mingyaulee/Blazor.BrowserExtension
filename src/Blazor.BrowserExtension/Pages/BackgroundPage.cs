using System.Threading.Tasks;

namespace Blazor.BrowserExtension.Pages
{
    public class BackgroundPage : BasePage
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await ImportAsync("BrowserExtensionScripts/Background.js");
        }
    }
}
