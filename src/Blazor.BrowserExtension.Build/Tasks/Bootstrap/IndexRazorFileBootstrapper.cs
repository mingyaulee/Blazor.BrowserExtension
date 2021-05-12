using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class IndexRazorFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Insert
            // @page "/index.html"
            // @inherits Blazor.BrowserExtension.Pages.IndexPage
            // after @page "/"
            var pageDirectiveIndex = fileLines.FindIndex(fileLine => fileLine.Contains("@page \"/index.html\""));
            if (pageDirectiveIndex == -1)
            {
                fileLines.Insert(1, "@page \"/index.html\"");
                fileLines.Insert(2, "@inherits Blazor.BrowserExtension.Pages.IndexPage");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
