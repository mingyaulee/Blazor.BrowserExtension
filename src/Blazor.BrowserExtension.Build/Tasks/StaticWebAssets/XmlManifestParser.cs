using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public class XmlManifestParser : BaseManifestParser
    {
        public XmlManifestParser(IEnumerable<string> excludePaths) : base(excludePaths)
        {
        }

        public override void ReadFromFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(XmlStaticWebAssetsRoot));
            using var reader = new StreamReader(filePath);
            var staticWebAssetsRoot = (XmlStaticWebAssetsRoot)serializer.Deserialize(reader);
            if (staticWebAssetsRoot?.ContentRoots is null || !staticWebAssetsRoot.ContentRoots.Any())
            {
                return;
            }

            foreach (var contentRoot in staticWebAssetsRoot.ContentRoots)
            {
                ParseXmlContentRoot(contentRoot);
            }
        }

        private void ParseXmlContentRoot(XmlContentRoot contentRoot)
        {
            if (contentRoot is null)
            {
                return;
            }

            var contentRootPath = NormalizePath(Path.GetFullPath(contentRoot.Path));
            if (ShouldExcludePath(contentRootPath))
            {
                return;
            }

            var basePath = contentRoot.BasePath;
            var contentFiles = Directory.GetFiles(contentRootPath, "*", SearchOption.AllDirectories);
            foreach (var contentFile in contentFiles)
            {
                var contentRelativeDirectory = basePath + Path.GetDirectoryName(contentFile).Replace(contentRootPath, string.Empty);
                AddOutput(contentFile, contentRelativeDirectory);
            }
        }
    }
}
