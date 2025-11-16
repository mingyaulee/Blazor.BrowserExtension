using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Playwright;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public abstract class BaseIntegrationTest
    {
        private static IPlaywright playwright;
        private static IPage page;
        private static readonly List<string> consoleMessages = [];
        protected static string extensionPath;
        protected static string extensionBaseUrl;

        [TestMethod]
        public async Task HomePageIsLoaded()
        {
            Assert.StartsWith("chrome-extension://", page.Url);
            Assert.AreEqual("Home", await GetPageContent());
        }

        [TestMethod]
        public async Task PopupPageIsLoaded()
        {
            await NavigateToUrl($"{extensionBaseUrl}/index.html?path=popup");
            Assert.AreEqual("Popup", await GetPageContent());
            Assert.AreEqual($"{extensionBaseUrl}/popup", page.Url);
        }

        [TestMethod]
        public async Task OptionsPageIsLoaded()
        {
            await NavigateToUrl($"{extensionBaseUrl}/index.html?path=options");
            Assert.AreEqual("Options", await GetPageContent());
            Assert.AreEqual($"{extensionBaseUrl}/options", page.Url);
        }

        [TestMethod]
        public async Task ContentScriptIsLoaded()
        {
            await NavigateToUrl("https://developer.chrome.com/");
            Assert.AreEqual("ContentScript", await GetPageContent(true));
        }

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static async Task InitializeAsync(TestContext testContext)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDirectory = currentDirectory[..currentDirectory.LastIndexOf("\\test")];
            var assembly = Assembly.GetExecutingAssembly();
            var targetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkDisplayName.ToLower().Replace(" ", string.Empty).TrimStart('.');
            var configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;
            extensionPath = @$"{solutionDirectory}\test\Blazor.BrowserExtension.IntegrationTest\bin\{configuration}\{targetFramework}\browserextension";

            if (testContext.FullyQualifiedTestClassName == typeof(IntegrationTestManifestV3).FullName)
            {
                File.Copy(Path.Combine(extensionPath, "manifestv3.json"), Path.Combine(extensionPath, "manifest.json"), true);
            }

            playwright = await Playwright.CreateAsync();
            var browser = await LaunchBrowser(playwright, currentDirectory, extensionPath);
            page = await browser.RunAndWaitForPageAsync(static () => Task.CompletedTask);
            page.Console += (_, message) => consoleMessages.Add(message.Text);
            extensionBaseUrl = page.Url[..^"/index.html".Length];
        }

        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void Cleanup()
        {
            playwright?.Dispose();
            playwright = null;
            page = null;
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

        private static async Task NavigateToUrl(string url)
        {
            consoleMessages.Clear();
            await page.GotoAsync(url);
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
