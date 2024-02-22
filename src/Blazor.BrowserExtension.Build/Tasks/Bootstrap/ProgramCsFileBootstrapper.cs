using System;
using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ProgramCsFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Wrap RootComponents setup
            // builder.UseBrowserExtension(environment =>
            // {
            //     builder.RootComponents.Add<App>("#app");
            //     builder.RootComponents.Add<HeadOutlet>("head::after");
            // });
            var useBrowserExtensionIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".UseBrowserExtension"));
            if (useBrowserExtensionIndex == -1)
            {
                var rootComponentsFirstIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".RootComponents."));

                if (rootComponentsFirstIndex == -1)
                {
                    throw new InvalidOperationException("Unable to find builder.RootComponents.Add<>() in Program.cs file.");
                }

                var indentCount = 0;
                foreach (var character in fileLines[rootComponentsFirstIndex])
                {
                    if (character != ' ')
                    {
                        break;
                    }
                    indentCount++;
                }

                var rootComponentsLastIndex = fileLines.FindLastIndex(fileLine => fileLine.Contains(".RootComponents."));
                var indent = "".PadLeft(indentCount, ' ');
                fileLines.Insert(rootComponentsFirstIndex + 0, $"{indent}builder.UseBrowserExtension(browserExtension =>");
                fileLines.Insert(rootComponentsFirstIndex + 1, $"{indent}{{");
                for (var i = rootComponentsFirstIndex + 2; i <= rootComponentsLastIndex + 2; i++)
                {
                    fileLines[i] = "    " + fileLines[i];
                }
                fileLines.Insert(rootComponentsLastIndex + 3, $"{indent}}});");

                if (!string.IsNullOrWhiteSpace(fileLines[rootComponentsFirstIndex - 1]))
                {
                    fileLines.Insert(rootComponentsFirstIndex - 1, string.Empty);
                }

                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
