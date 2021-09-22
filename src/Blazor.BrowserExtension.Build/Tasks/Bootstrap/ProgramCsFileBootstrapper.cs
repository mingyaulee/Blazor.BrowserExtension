using System;
using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ProgramCsFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Insert
            // builder.Services.AddBrowserExtensionServices();
            // before await builder.Build().RunAsync();
            var registerServicesIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".AddBrowserExtensionServices"));
            if (registerServicesIndex == -1)
            {
                var buildHostIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".Build().RunAsync()"));
                if (buildHostIndex == -1)
                {
                    throw new InvalidOperationException("Unable to find builder.Build().RunAsync() in Program.cs file.");
                }

                var indentCount = 0;
                foreach (var character in fileLines[buildHostIndex])
                {
                    if (character != ' ')
                    {
                        break;
                    }
                    indentCount++;
                }

                var indent = "".PadLeft(indentCount, ' ');
                fileLines.Insert(buildHostIndex + 0, $"{indent}builder.Services.AddBrowserExtensionServices();");
                fileLines.Insert(buildHostIndex + 1, "");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
