using System;
using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ProgramCsFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Add
            // using Blazor.BrowserExtension;
            var usingStatement = "using Blazor.BrowserExtension;";
            var usingStatementIndex = fileLines.FindIndex(fileLine => fileLine.Contains(usingStatement));
            if (usingStatementIndex == -1)
            {
                fileLines.Insert(0, usingStatement);
                isUpdated = true;
            }

            // Wrap RootComponents setup
            // builder.UseBrowserExtension(environment =>
            // {
            //     if (browserExtension.Mode == BrowserExtensionMode.Background)
            //     {
            //         builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();
            //     }
            //     else
            //     {
            //         builder.RootComponents.Add<App>("#app");
            //         builder.RootComponents.Add<HeadOutlet>("head::after");
            //     }
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
                fileLines.Insert(rootComponentsFirstIndex + 2, $"{indent}    if (browserExtension.Mode == BrowserExtensionMode.Background)");
                fileLines.Insert(rootComponentsFirstIndex + 3, $"{indent}    {{");
                fileLines.Insert(rootComponentsFirstIndex + 4, $"{indent}        builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();");
                fileLines.Insert(rootComponentsFirstIndex + 5, $"{indent}    }}");
                fileLines.Insert(rootComponentsFirstIndex + 6, $"{indent}    else");
                fileLines.Insert(rootComponentsFirstIndex + 7, $"{indent}    {{");
                for (var i = rootComponentsFirstIndex + 8; i <= rootComponentsLastIndex + 8; i++)
                {
                    fileLines[i] = "        " + fileLines[i];
                }
                fileLines.Insert(rootComponentsLastIndex + 9, $"{indent}    }}");
                fileLines.Insert(rootComponentsLastIndex + 10, $"{indent}}});");

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
