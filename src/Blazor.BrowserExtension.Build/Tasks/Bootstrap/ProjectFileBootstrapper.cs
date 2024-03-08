using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ProjectFileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Remove <BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>
            var bootstrapPropertyTagIndex = fileLines.FindIndex(fileLine => fileLine.Contains("BrowserExtensionBootstrap"));
            if (bootstrapPropertyTagIndex > -1)
            {
                fileLines.RemoveAt(bootstrapPropertyTagIndex);
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
