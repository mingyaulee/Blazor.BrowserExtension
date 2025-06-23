﻿using Microsoft.Playwright;

namespace Blazor.BrowserExtension.Build.Test.Helpers
{
    public class WebDriverExtensionHelper : IDisposable
    {
        private IPlaywright playwright;
        private IPage page;
        protected string extensionPath;
        protected string extensionBaseUrl;
        private bool disposedValue;

        public string CurrentUrl => page.Url;
        public string ExtensionBaseUrl { get; private set; }

        public async Task Load(string extensionPath)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            playwright = await Playwright.CreateAsync();
            var browser = await LaunchBrowser(playwright, currentDirectory, extensionPath);
            page = await browser.RunAndWaitForPageAsync(static () => Task.CompletedTask);
            var consoleMessages = new List<string>();
            page.Console += (_, message) =>
            {
                consoleMessages.Add(message.Text);
            };

            ExtensionBaseUrl = page.Url[..^"/index.html".Length];
        }

        public Task NavigateToUrl(string url)
        {
            return page.GotoAsync(url);
        }

        public string GetExtensionUrl(string path)
        {
            return $"{ExtensionBaseUrl}index.html?path={path}";
        }

        public async Task<string> GetContent(string selector)
        {
            await page.WaitForSelectorAsync(selector);
            return await page.EvalOnSelectorAsync<string>(selector, "el => el.innerText");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    playwright.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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
    }
}
