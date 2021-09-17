using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blazor.BrowserExtension.Build.Tasks.StaticWebAssets;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionProcessStaticWebAssetsManifest : Task
    {
        [Required]
        public ITaskItem Input { get; set; }
        public ITaskItem[] Exclude { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var excludePaths = Exclude?.Select(exclude => exclude.ItemSpec);
            Output = ReadFromFile(Input.ItemSpec, excludePaths);
            return true;
        }

        private static ITaskItem[] ReadFromFile(string filePath, IEnumerable<string> excludePaths)
        {
            BaseManifestParser parser;
            if (".xml".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
            {
                parser = new XmlManifestParser(excludePaths);
            }
            else
            {
                parser = new JsonManifestParser(excludePaths);
            }
            parser.ReadFromFile(filePath);
            var output = parser.GetOutput()
                .Select(staticWebAssetFile =>
                    new TaskItem(staticWebAssetFile.FilePath, new Dictionary<string, string>()
                    {
                        { "ContentRelativeDirectory", staticWebAssetFile.RelativePath }
                    })
                ).ToArray();
            return output;
        }
    }
}
