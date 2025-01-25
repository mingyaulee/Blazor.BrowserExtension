using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks.Helpers
{
    internal static partial class FingerprintHelper
    {

#if NET7_0_OR_GREATER
        [GeneratedRegex(@"#\[.+\][!\?]?")]
        private static partial Regex GetFingerprintRegex();
#else
        private static Regex GetFingerprintRegex() => new(@"#\[.+\][!\?]?");
#endif

        public static string RemoveFingerprintSegment(string path)
        {
            return GetFingerprintRegex().Replace(path, string.Empty);
        }
    }
}
