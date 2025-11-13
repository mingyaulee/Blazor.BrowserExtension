using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public abstract class BaseIntegrationTest : IAsyncLifetime
    {
        private IPlaywright playwright;
        private IPage page;
        private readonly List<string> consoleMessages = [];
        protected string extensionPath;
        protected string extensionBaseUrl;

        [Fact]
        public async Task HomePageIsLoaded()
        {
            Assert.StartsWith("chrome-extension://", page.Url);
            Assert.Equal("Home", await GetPageContent());
        }

        [Fact]
        public async Task PopupPageIsLoaded()
        {
            await NavigateToUrl($"{extensionBaseUrl}/index.html?path=popup");
            Assert.Equal("Popup", await GetPageContent());
            Assert.Equal($"{extensionBaseUrl}/popup", page.Url);
        }

        [Fact]
        public async Task OptionsPageIsLoaded()
        {
            await NavigateToUrl($"{extensionBaseUrl}/index.html?path=options");
            Assert.Equal("Options", await GetPageContent());
            Assert.Equal($"{extensionBaseUrl}/options", page.Url);
        }

        [Fact]
        public async Task ContentScriptIsLoaded()
        {
            await NavigateToUrl("https://developer.chrome.com/");
            Assert.Equal("ContentScript", await GetPageContent(true));
        }

        protected abstract void SetupBeforeInitialize();

        public virtual async Task InitializeAsync()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDirectory = currentDirectory[..currentDirectory.LastIndexOf("\\test")];
            var assembly = Assembly.GetExecutingAssembly();
            var targetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkDisplayName.ToLower().Replace(" ", string.Empty).TrimStart('.');
            var configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;
            extensionPath = @$"{solutionDirectory}\test\Blazor.BrowserExtension.IntegrationTest\bin\{configuration}\{targetFramework}\browserextension";

            SetupBeforeInitialize();
            playwright = await Playwright.CreateAsync();
            var browser = await LaunchBrowser(playwright, currentDirectory, extensionPath);
            page = await browser.RunAndWaitForPageAsync(static () => Task.CompletedTask);
            page.Console += (_, message) => consoleMessages.Add(message.Text);
            extensionBaseUrl = page.Url[..^"/index.html".Length];
        }

        public Task DisposeAsync()
        {
            playwright.Dispose();
            return Task.CompletedTask;
        }

        private static Task<IBrowserContext> LaunchBrowser(IPlaywright playwright, string currentDirectory, string extensionPath)
        {
            var userDataDir = Path.Combine(currentDirectory, "chrome");
            if (Directory.Exists(userDataDir))
            {
                Directory.Delete(userDataDir, true);
            }
            Directory.CreateDirectory(userDataDir);

            return playwright.Chromium.LaunchPersistentContextAsync(userDataDir, new()
            {
                Headless = false,
                Channel = "chromium",
                Args =
                [
                    $"--disable-extensions-except={extensionPath}",
                    $"--load-extension={extensionPath}"
                ]
            });
        }

        private Task NavigateToUrl(string url)
        {
            consoleMessages.Clear();
            return page.GotoAsync(url);
        }

        private async Task<string> GetPageContent(bool isContentScript = false)
        {
            var appId = isContentScript ? "#Blazor_BrowserExtension_IntegrationTest_app h3" : "#app h3";
            try
            {
                await page.WaitForSelectorAsync(appId);
            }
            catch(TimeoutException)
            {
                Assert.Fail($"Failed to get content for '{appId}'. Console messages: {string.Join(Environment.NewLine, consoleMessages)}");
            }
            return await page.EvalOnSelectorAsync<string>(appId, "el => el.innerText");
        }
    }
}
