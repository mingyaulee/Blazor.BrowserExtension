using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ImportsRazorFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Add
            // @using Blazor.BrowserExtension.Pages
            var usingStatement = "@using Blazor.BrowserExtension.Pages";
            var usingStatementIndex = fileLines.FindIndex(fileLine => fileLine.Contains(usingStatement));
            if (usingStatementIndex == -1)
            {
                fileLines.Insert(fileLines.FindLastIndex(line => !string.IsNullOrEmpty(line)) + 1, usingStatement);
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
