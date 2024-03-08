using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class IndexRazorFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Replace
            // @page "/"
            // with
            // @page "/index.html"
            // @inherits IndexPage
            var pageDirectiveIndex = fileLines.FindIndex(fileLine => fileLine.Contains("@page \"/\""));
            if (pageDirectiveIndex > -1)
            {
                fileLines[pageDirectiveIndex] = "@page \"/index.html\"";
                fileLines.Insert(pageDirectiveIndex + 1, "@inherits IndexPage");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
