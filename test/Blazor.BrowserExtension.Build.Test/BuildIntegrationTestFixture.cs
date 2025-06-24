using Blazor.BrowserExtension.Build.Test.Helpers;

namespace Blazor.BrowserExtension.Build.Test
{
    public class BuildIntegrationTestFixture : IDisposable
    {
        private const string DotNetCommand = "dotnet";
        private readonly string rootTestDirectory;
        private readonly string rootSolutionDirectory;
        private bool disposedValue;

#if DEBUG
        const string CurrentConfiguration = "Debug";
#else
        const string CurrentConfiguration = "Release";
#endif

        public BuildIntegrationTestFixture()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            rootTestDirectory = currentDirectory[..currentDirectory.LastIndexOf(Path.DirectorySeparatorChar + "bin")];
            rootSolutionDirectory = rootTestDirectory[..rootTestDirectory.LastIndexOf(Path.DirectorySeparatorChar + "test")];

            var packagesCacheDirectory = Path.Combine(rootSolutionDirectory, "PackagesCache");
            if (Directory.Exists(packagesCacheDirectory))
            {
                Directory.Delete(packagesCacheDirectory, true);
            }

            CommandHelper.ExecuteCommandVoid(DotNetCommand, $"pack --no-build --no-restore --configuration {CurrentConfiguration}", rootSolutionDirectory);
            try
            {
                CommandHelper.ExecuteCommandVoid(DotNetCommand, $"new install src/PackageOutput/Blazor.BrowserExtension.Template.1.0.0.nupkg", rootSolutionDirectory);
            }
            catch (Exception exception) when (exception.Message.Contains("already exists"))
            {
                // Ignore exception when template is already installed
            }
        }

        public void ExecuteDotnetCommand(string command)
        {
            CommandHelper.ExecuteCommandVoid(DotNetCommand, command, Path.Combine(rootTestDirectory, "TestProjects"));
        }

        public void ExecuteDotnetCommand(string command, string testProjectName)
        {
            CommandHelper.ExecuteCommandVoid(DotNetCommand, command, GetTestProjectDirectory(testProjectName));
        }

        public void ExecuteDotnetRestoreCommand(string testProjectName)
            => ExecuteDotnetCommand("restore --no-cache", testProjectName);

        public void ExecuteDotnetBuildCommand(string testProjectName)
            => ExecuteDotnetCommand("build --no-restore", testProjectName);

        public void ExecuteDotnetPublishCommand(string testProjectName)
            => ExecuteDotnetCommand("publish --no-restore", testProjectName);

        public string GetTestProjectDirectory(string testProjectName)
            => Path.Combine(rootTestDirectory, "TestProjects", testProjectName);

        public Task<WebDriverExtensionHelper> LoadExtensionBuildOutput(string testProjectName)
        {
            var extensionPath = Path.Combine(GetTestProjectDirectory(testProjectName), "bin", "Debug", "net9.0", "browserextension");
            return LoadExtension(extensionPath);
        }

        public Task<WebDriverExtensionHelper> LoadExtensionPublishOutput(string testProjectName)
        {
            var extensionPath = Path.Combine(GetTestProjectDirectory(testProjectName), "bin", "Release", "net9.0", "publish", "browserextension");
            return LoadExtension(extensionPath);
        }

        private static async Task<WebDriverExtensionHelper> LoadExtension(string extensionPath)
        {
            var extension = new WebDriverExtensionHelper();
            await extension.Load(extensionPath);
            return extension;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CommandHelper.ExecuteCommandVoid(DotNetCommand, $"new uninstall Blazor.BrowserExtension.Template", rootSolutionDirectory);
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
