using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace Blazor.BrowserExtension.Build.Test.Helpers
{
    public class WebDriverExtensionHelper : IDisposable
    {
        public readonly WebDriver WebDriver;
        private bool disposedValue;

        public string CurrentUrl => WebDriver.Url;
        public string ExtensionBaseUrl { get; private set; }

        public WebDriverExtensionHelper(WebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public Task NavigateToUrl(string url)
        {
            WebDriver.Url = url;
            return Retry(() => (bool)WebDriver.ExecuteScript("return document.readyState === \"complete\";"), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }

        public string GetExtensionUrl(string path)
        {
            return $"{ExtensionBaseUrl}index.html?path={path}";
        }

        public async Task WaitForExtensionPage()
        {
            var isLoaded = await Retry(
                () => WebDriver.WindowHandles.Count > 1,
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(10));
            isLoaded.Should().BeTrue();

            // switch to the second window
            WebDriver.SwitchTo().Window(WebDriver.WindowHandles[1]);
            ExtensionBaseUrl = CurrentUrl.Substring(0, CurrentUrl.LastIndexOf("/") + 1);
            CurrentUrl.Should().StartWith("chrome-extension://");
        }

        public async Task<string> GetContent(string selector)
        {
            await Retry(
                () => (bool)WebDriver.ExecuteScript($"""return document.querySelector("{selector}") != null;"""),
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return WebDriver.FindElement(By.CssSelector($"{selector}"))?.Text;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WebDriver.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
