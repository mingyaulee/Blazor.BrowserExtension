using System;
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

            var includeWwwrootIndex = fileLines.FindIndex(fileLine => fileLine.Contains(@"wwwroot\**\*"));
            if (includeWwwrootIndex == -1)
            {
                // Insert
                // <ItemGroup>
                //   <None Include="wwwroot\**\*" CopyToOutputDirectory="Always" />
                // </ItemGroup>
                // before </Project>
                var projectCloseTagIndex = fileLines.FindIndex(fileLine => fileLine.Contains("</Project>"));
                if (projectCloseTagIndex == -1)
                {
                    throw new InvalidOperationException("Unable to find project closing tag </Project> in project file.");
                }
                fileLines.Insert(projectCloseTagIndex + 0, "  <ItemGroup>");
                fileLines.Insert(projectCloseTagIndex + 1, @"    <None Include=""wwwroot\**\*"" CopyToOutputDirectory=""Always"" />");
                fileLines.Insert(projectCloseTagIndex + 2, "  </ItemGroup>");
                fileLines.Insert(projectCloseTagIndex + 3, "");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
