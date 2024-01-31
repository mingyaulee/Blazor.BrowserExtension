using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class IndexHtmlFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Replace the script tag `<script src="_framework/blazor.webassembly.js"></script>` with `<script src="framework/blazor.webassembly.js"></script>`
            var scriptTagIndex = fileLines.FindIndex(fileLine => fileLine.Contains("_framework/blazor.webassembly.js"));
            if (scriptTagIndex > -1)
            {
                fileLines[scriptTagIndex] = fileLines[scriptTagIndex].Replace("_framework/blazor.webassembly.js", "framework/blazor.webassembly.js");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
