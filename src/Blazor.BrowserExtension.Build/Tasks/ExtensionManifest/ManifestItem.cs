using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestItem(long? lineNumber, long? columnNumber, string value)
    {
        public long? LineNumber { get; } = lineNumber;
        public long? ColumnNumber { get; set; } = columnNumber;
        public string Value { get; } = value;

        public bool ContainsExact(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""));
        public bool ContainsExactIgnoreCase(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""), RegexOptions.IgnoreCase);
    }
}
