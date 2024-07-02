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
                throw new NotSupportedException($"Download the chromedriver from and extract the executable file to {driverPath}. Download the latest version from https://googlechromelabs.github.io/chrome-for-testing/");
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
                throw new NotSupportedException("Failed to create WebDriver. Download the latest version from https://googlechromelabs.github.io/chrome-for-testing/\"", exception);
            }
            webDriverHelper = new WebDriverHelper(webDriver);
        }

        public static string GetExtensionPath()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDirectory = currentDirectory[..currentDirectory.LastIndexOf("\\test")];
#if DEBUG
            var configuration = "debug";
#else
            var configuration = "release";
#endif
            return $"{solutionDirectory}\\test\\Blazor.BrowserExtension.IntegrationTest\\bin\\{configuration}\\net8.0\\browserextension";
        }

        private static ChromeDriver GetWebDriver(string driverPath, string extensionPath)
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
