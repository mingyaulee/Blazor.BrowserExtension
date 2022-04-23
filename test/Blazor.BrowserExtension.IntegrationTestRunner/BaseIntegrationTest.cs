using System;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    [TestCaseOrderer("Blazor.BrowserExtension.IntegrationTestRunner.TestOrderer", "Blazor.BrowserExtension.IntegrationTestRunner")]
    public abstract class BaseIntegrationTest : IClassFixture<Fixture>
    {
        private readonly WebDriverHelper webDriverHelper;

        public BaseIntegrationTest(Fixture fixture)
        {
            if (!fixture.IsInitialized)
            {
                SetupBeforeInitialize();
                fixture.Initialize();
            }
            webDriverHelper = fixture.WebDriverHelper;
        }

        protected abstract void SetupBeforeInitialize();

        [Fact, Order(1)]
        public async Task IndexPageIsLoaded()
        {
            // The background page creates a new tab with the url of the extension index page on load, so check on the count of window handles
            var isLoaded = await webDriverHelper.Retry(
                () => webDriverHelper.WebDriver.WindowHandles.Count > 1,
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(10));
            Assert.True(isLoaded);

            // switch to the second window
            webDriverHelper.WebDriver.SwitchTo().Window(webDriverHelper.WebDriver.WindowHandles[1]);
            webDriverHelper.SetExtensionBaseUrl(webDriverHelper.CurrentUrl);

            Assert.StartsWith("chrome-extension://", webDriverHelper.CurrentUrl);
            Assert.Equal("Index", await webDriverHelper.GetPageContent());
        }

        [Fact, Order(2)]
        public async Task PopupPageIsLoaded()
        {
            await webDriverHelper.NavigateToUrl(webDriverHelper.GetExtensionUrl("popup"));
            Assert.Equal("Popup", await webDriverHelper.GetPageContent());
            Assert.Equal($"{webDriverHelper.ExtensionBaseUrl}popup", webDriverHelper.CurrentUrl);
        }

        [Fact, Order(3)]
        public async Task OptionsPageIsLoaded()
        {
            await webDriverHelper.NavigateToUrl(webDriverHelper.GetExtensionUrl("options"));
            Assert.Equal("Options", await webDriverHelper.GetPageContent());
            Assert.Equal($"{webDriverHelper.ExtensionBaseUrl}options", webDriverHelper.CurrentUrl);
        }

        [Fact, Order(4)]
        public async Task ContentScriptIsLoaded()
        {
            await webDriverHelper.NavigateToUrl($"https://developer.chrome.com/");
            Assert.Equal("ContentScript", await webDriverHelper.GetPageContent());
        }
    }
}
