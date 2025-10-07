using Blazor.BrowserExtension.Build.Test.Helpers;
using Xunit;

namespace Blazor.BrowserExtension.Build.Test
{
    public class BuildIntegrationTest(BuildIntegrationTestFixture testFixture) : IClassFixture<BuildIntegrationTestFixture>
    {
        [Fact]
        public async Task TestNewProjectFromTemplate()
        {
            var projectName = "NewBrowserExtensionProject";
            var projectDirectory = testFixture.GetTestProjectDirectory(projectName);
            testFixture.ExecuteDotnetCommand($"new browserext --name {projectName}");
            try
            {
                testFixture.ExecuteDotnetRestoreCommand(projectName);
                testFixture.ExecuteDotnetBuildCommand(projectName);
                using (var extensionFromBuild = await testFixture.LoadExtensionBuildOutput(projectName))
                {
                    var pageContentFromBuild = await extensionFromBuild.GetContent("h1");
                    pageContentFromBuild.ShouldBe("Hello, from Blazor.");
                }

                testFixture.ExecuteDotnetPublishCommand(projectName);
                using var extensionFromPublish = await testFixture.LoadExtensionPublishOutput(projectName);
                var pageContentFromPublish = await extensionFromPublish.GetContent("h1");
                pageContentFromPublish.ShouldBe("Hello, from Blazor.");
            }
            finally
            {
                ResetDirectoryChanges(projectDirectory);
            }
        }

        [Fact]
        public async Task TestBootstrapExistingProject()
        {
            var projectName = "EmptyBlazorProject";
            var projectDirectory = testFixture.GetTestProjectDirectory(projectName);
            try
            {
                var projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
                var projectFileContent = await File.ReadAllTextAsync(projectFile);
                projectFileContent = projectFileContent
                    .Replace("[NetVersion]", CommonTestHelper.TargetFrameworkMajorVersion);
                await File.WriteAllTextAsync(projectFile, projectFileContent);

                testFixture.ExecuteDotnetRestoreCommand(projectName);
                testFixture.ExecuteDotnetBuildCommand(projectName);
                projectFileContent = await File.ReadAllTextAsync(projectFile);
                projectFileContent.ShouldNotContain("BrowserExtensionBootstrap");
                using (var extensionFromBuild = await testFixture.LoadExtensionBuildOutput(projectName))
                {
                    var pageContentFromBuild = await extensionFromBuild.GetContent("h1");
                    pageContentFromBuild.ShouldBe("Hello, world!");
                }

                testFixture.ExecuteDotnetPublishCommand(projectName);
                using var extensionFromPublish = await testFixture.LoadExtensionPublishOutput(projectName);
                var pageContentFromPublish = await extensionFromPublish.GetContent("h1");
                pageContentFromPublish.ShouldBe("Hello, world!");
            }
            finally
            {
                ResetDirectoryChanges(projectDirectory);
            }
        }

        static void ResetDirectoryChanges(string directory)
        {
            try
            {
                CommandHelper.ExecuteCommandVoid("git", "restore .", directory);
            }
            catch (Exception exception) when (exception.Message.Contains("pathspec '.' did not match any file(s) known to git"))
            {
                // No file to restore which is fine
            }
            CommandHelper.ExecuteCommandVoid("git", "clean . -xdf", directory);
        }
    }
}
