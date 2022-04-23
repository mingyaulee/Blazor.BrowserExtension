using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class Fixture : IDisposable
    {
        private bool disposedValue;
        private bool isInitialized;
        private WebDriver webDriver;
        private WebDriverHelper webDriverHelper;
        public bool IsInitialized => isInitialized;
        public WebDriver WebDriver => webDriver;
        public WebDriverHelper WebDriverHelper => webDriverHelper;

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            var driverPath = "C:\\SeleniumWebDrivers\\ChromeDriver";
            if (!Directory.Exists(driverPath))
            {
                throw new NotSupportedException($"Download the chromedriver from and extract the executable file to {driverPath}. Check available versions at http://chromedriver.storage.googleapis.com/");
            }

            var extensionPath = GetExtensionPath();
            if (!Directory.Exists(extensionPath))
            {
                throw new NotSupportedException($"Extension does not exist in path {extensionPath}.");
            }

            try
            {
                webDriver = GetWebDriver(driverPath, extensionPath);
            }
            catch (Exception exception)
            {
                throw new NotSupportedException("Failed to create WebDriver.", exception);
            }
            webDriverHelper = new WebDriverHelper(webDriver);
        }

        public static string GetExtensionPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("\\test"));
#if DEBUG
            var configuration = "debug";
#else
            var configuration = "release";
#endif
            return $"{solutionDirectory}\\test\\Blazor.BrowserExtension.IntegrationTest\\bin\\{configuration}\\net7.0\\browserextension";
        }

        private static WebDriver GetWebDriver(string driverPath, string extensionPath)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument($"load-extension={extensionPath}");
            return new ChromeDriver(driverPath, chromeOptions);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                webDriver?.Close();
                webDriver?.Dispose();
                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
