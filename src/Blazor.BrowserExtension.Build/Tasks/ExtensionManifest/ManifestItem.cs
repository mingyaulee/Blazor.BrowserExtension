using System.Text;
using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestItem
    {
        private readonly StringBuilder valueStringBuilder;

        public ManifestItem(int lineNumber)
        {
            valueStringBuilder = new StringBuilder();
            LineNumber = lineNumber;
        }

        public int LineNumber { get; }
        private string value;
        public string Value
        {
            get
            {
                if (value is null)
                {
                    value = valueStringBuilder.ToString();
                }
                return value;
            }
        }

        public void AddValue(string value)
        {
            this.value = null;
            valueStringBuilder.Append(value);
        }

        public bool ContainsExact(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""));

        public bool ContainsExactIgnoreCase(string value) => Regex.IsMatch(Value, Regex.Escape($"\"{value}\""), RegexOptions.IgnoreCase);
    }
}
