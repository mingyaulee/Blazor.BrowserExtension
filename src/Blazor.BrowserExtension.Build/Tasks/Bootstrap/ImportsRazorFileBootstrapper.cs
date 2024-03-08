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
            var insertLine = "@using Blazor.BrowserExtension.Pages";
            var bootstrapPropertyTagIndex = fileLines.FindIndex(fileLine => fileLine.Contains("@using Blazor.BrowserExtension.Pages"));
            if (bootstrapPropertyTagIndex == -1)
            {
                fileLines.Insert(fileLines.FindLastIndex(line => !string.IsNullOrEmpty(line)) + 1, insertLine);
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
