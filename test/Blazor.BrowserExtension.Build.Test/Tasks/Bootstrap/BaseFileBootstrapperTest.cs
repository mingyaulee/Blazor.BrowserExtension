using System;
using System.Linq;
using Blazor.BrowserExtension.Build.Tasks.Bootstrap;
using FluentAssertions;
using Xunit;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    public abstract class BaseFileBootstrapperTest<TBootstrapper>
        where TBootstrapper : IFileBootstrapper, new()
    {
        protected abstract string OriginalFileContent { get; }
        protected abstract string BootstrappedFileContent { get; }

        [Fact]
        public void TestBootstrap()
        {
            // Arrange
            var bootstrapper = new TBootstrapper();
            var fileLines = OriginalFileContent.Split(Environment.NewLine).ToList();
            var expectedFileLines = BootstrappedFileContent.Split(Environment.NewLine).ToList();

            // Act
            var isUpdated = bootstrapper.Bootstrap(fileLines);

            // Assert
            isUpdated.Should().BeTrue();
            fileLines.Should().BeEquivalentTo(expectedFileLines);
        }

        [Fact]
        public void TestBootstrapWithBootstrappedFile()
        {
            // Arrange
            var bootstrapper = new TBootstrapper();
            var fileLines = BootstrappedFileContent.Split(Environment.NewLine).ToList();
            var expectedFileLines = BootstrappedFileContent.Split(Environment.NewLine).ToList();

            // Act
            var isUpdated = bootstrapper.Bootstrap(fileLines);

            // Assert
            isUpdated.Should().BeFalse();
            fileLines.Should().BeEquivalentTo(expectedFileLines);
        }
    }
}
