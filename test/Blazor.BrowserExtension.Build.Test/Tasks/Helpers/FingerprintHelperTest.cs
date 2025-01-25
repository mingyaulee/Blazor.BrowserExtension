using Blazor.BrowserExtension.Build.Tasks.Helpers;
using FluentAssertions;
using Xunit;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Helpers
{
    public class FingerprintHelperTest
    {
        [Theory]
        [InlineData("app#[.{fingerprint}]?.js", "app.js")]
        public void TestRemoveFingerprintSegment(string input, string expected)
        {
            // Act
            var actual = FingerprintHelper.RemoveFingerprintSegment(input);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
