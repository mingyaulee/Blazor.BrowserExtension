using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Blazor.BrowserExtension.Build.Tasks.StaticWebAssets;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionProcessStaticWebAssetsManifest : Task
    {
        [Required]
        public ITaskItem[] Input { get; set; }
        public ITaskItem[] Exclude { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var output = new List<ITaskItem>();
            var excludePaths = Exclude?.Select(exclude => NormalizePath(exclude.ItemSpec)).ToArray() ?? Enumerable.Empty<string>();
            var serializer = new XmlSerializer(typeof(StaticWebAssetsRoot));
            foreach (var input in Input)
            {
                StreamReader reader = new StreamReader(input.ItemSpec);
                var staticWebAssetsRoot = (StaticWebAssetsRoot)serializer.Deserialize(reader);
                reader.Close();
                ParseManifestXml(staticWebAssetsRoot, output, excludePaths);
            }
            Output = output.ToArray();
            return true;
        }

        private static void ParseManifestXml(StaticWebAssetsRoot staticWebAssetsRoot, List<ITaskItem> output, IEnumerable<string> excludePaths)
        {
            if (staticWebAssetsRoot?.ContentRoots is null)
            {
                return;
            }

            foreach (var contentRoot in staticWebAssetsRoot.ContentRoots)
            {
                ParseContentRoot(contentRoot, output, excludePaths);
            }
        }

        private static void ParseContentRoot(ContentRoot contentRoot, List<ITaskItem> output, IEnumerable<string> excludePaths)
        {
            if (contentRoot is null)
            {
                return;
            }

            var contentRootPath = NormalizePath(contentRoot.Path);
            if (excludePaths.Any(excludePath => contentRootPath.Equals(excludePath, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var basePath = contentRoot.BasePath.Replace('/', Path.DirectorySeparatorChar);
            var contentFiles = Directory.GetFiles(contentRootPath, "*", SearchOption.AllDirectories);
            foreach (var contentFile in contentFiles)
            {
                var contentRelativeDirectory = (basePath + Path.GetDirectoryName(contentFile).Replace(contentRootPath, string.Empty)).TrimStart(Path.DirectorySeparatorChar, '_');
                output.Add(new TaskItem(contentFile, new Dictionary<string, string>()
                {
                    { "ContentRelativeDirectory", contentRelativeDirectory }
                }));
            }
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
