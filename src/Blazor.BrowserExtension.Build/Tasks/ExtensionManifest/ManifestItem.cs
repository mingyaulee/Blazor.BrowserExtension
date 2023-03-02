using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestItem
    {
        public ManifestItem(long? lineNumber, long? columnNumber, string value)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Value = value;
        }

        public long? LineNumber { get; }
        public long? ColumnNumber { get; set; }
        public string Value { get; }

        public bool ContainsExact(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""));
        public bool ContainsExactIgnoreCase(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""), RegexOptions.IgnoreCase);
    }
}
