using OpenQA.Selenium.Remote;
using System;
using System.Threading.Tasks;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class WebDriverHelper
    {
        public readonly RemoteWebDriver WebDriver;
        public string CurrentUrl => WebDriver.Url;
        public string ExtensionBaseUrl { get; private set; }

        public WebDriverHelper(RemoteWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public Task NavigateToUrl(string url)
        {
            WebDriver.Url = url;
            return Retry(() => (bool)WebDriver.ExecuteScript("return document.readyState === \"complete\";"), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }

        public void SetExtensionBaseUrl(string url)
        {
            ExtensionBaseUrl = url.Substring(0, url.LastIndexOf("/") + 1);
        }

        public string GetExtensionUrl(string path)
        {
            return $"{ExtensionBaseUrl}index.html?path={path}";
        }

        public async Task<string> GetPageContent()
        {
            await Retry(
                () => (bool)WebDriver.ExecuteScript("return document.querySelector(\"#Blazor_BrowserExtension_IntegrationTest_app h3\") != null;"),
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            return WebDriver.FindElementByCssSelector("#Blazor_BrowserExtension_IntegrationTest_app h3")?.Text;
        }

        public async Task<bool> Retry(Func<bool> action, TimeSpan interval, TimeSpan timeout)
        {
            var retryCount = timeout.TotalMilliseconds / interval.TotalMilliseconds;
            while (retryCount > 0)
            {
                retryCount--;
                if (action())
                {
                    return true;
                }
                await Task.Delay(interval);
            }
            return false;
        }
    }
}
