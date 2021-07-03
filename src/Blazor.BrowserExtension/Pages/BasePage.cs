using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using WebExtensions.Net;

namespace Blazor.BrowserExtension.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        public IWebExtensionsApi WebExtensions { get; set; }

        [Inject]
        public ILogger<BasePage> Logger { get; set; }
    }
}
