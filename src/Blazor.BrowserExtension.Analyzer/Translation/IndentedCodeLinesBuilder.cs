using System;
using System.Text;

namespace Blazor.BrowserExtension.Analyzer.Translation
{
    internal class IndentedCodeLinesBuilder(string indentation, string separator)
    {
        private bool isEmpty = true;
        private readonly StringBuilder stringBuilder = new();
        private const string NewLine = @"
";

        public void Append(string lines)
        {
            if (!isEmpty)
            {
                stringBuilder.Append(separator);
            }

            foreach (var line in lines.Split(NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (!isEmpty)
                {
                    stringBuilder.Append(NewLine);
                }

                stringBuilder.Append(indentation);
                stringBuilder.Append(line);
                isEmpty = false;
            }
        }

        public string Build()
        {
            return stringBuilder.ToString();
        }
    }
}
