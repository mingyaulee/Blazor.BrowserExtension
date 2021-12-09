using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.IO;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class Fixture : IDisposable
    {
        public RemoteWebDriver WebDriver { get; }
        public WebDriverHelper WebDriverHelper { get; }
        private bool disposedValue;

        public Fixture()
        {
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
                WebDriver = GetWebDriver(driverPath, extensionPath);
            }
            catch (Exception exception)
            {
                throw new NotSupportedException("Failed to create WebDriver.", exception);
            }
            WebDriverHelper = new WebDriverHelper(WebDriver);
        }

        private static string GetExtensionPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("\\test"));
#if DEBUG
            var configuration = "debug";
#else
            var configuration = "release";
#endif
            return $"{solutionDirectory}\\test\\Blazor.BrowserExtension.IntegrationTest\\bin\\{configuration}\\net6.0\\browserextension";
        }

        private static RemoteWebDriver GetWebDriver(string driverPath, string extensionPath)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument($"load-extension={extensionPath}");
            return new ChromeDriver(driverPath, chromeOptions);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                WebDriver?.Close();
                WebDriver?.Dispose();
                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
