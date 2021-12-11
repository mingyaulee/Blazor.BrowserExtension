#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public class XmlManifestProcessor : BaseManifestProcessor
    {
        private XmlStaticWebAssetsRoot? staticWebAssetsRoot;
        private bool isUpdated;

        public XmlManifestProcessor(IEnumerable<string> excludePaths) : base(excludePaths)
        {
        }

        public override void ReadFromFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(XmlStaticWebAssetsRoot));
            using var reader = new StreamReader(filePath);
            staticWebAssetsRoot = serializer.Deserialize(reader) as XmlStaticWebAssetsRoot;
            if (staticWebAssetsRoot?.ContentRoots is null || !staticWebAssetsRoot.ContentRoots.Any())
            {
                return;
            }

            foreach (var contentRoot in staticWebAssetsRoot.ContentRoots)
            {
                ParseXmlContentRoot(contentRoot);
            }
        }
        public override void Process(string outputPath)
        {
            if (staticWebAssetsRoot?.ContentRoots is null || !staticWebAssetsRoot.ContentRoots.Any())
            {
                return;
            }

            var frameworkPath = Path.Combine(Path.GetFullPath(outputPath), "framework");
            if (!staticWebAssetsRoot.ContentRoots.Any(contentRoot => contentRoot.Path == frameworkPath))
            {
                staticWebAssetsRoot.ContentRoots.Insert(0, new XmlContentRoot()
                {
                    BasePath = "framework",
                    Path = frameworkPath
                });
                isUpdated = true;
            }

            foreach (var contentRoot in staticWebAssetsRoot.ContentRoots.Where(contentRoot => contentRoot.BasePath.StartsWith("_content", StringComparison.OrdinalIgnoreCase)))
            {
                contentRoot.BasePath = contentRoot.BasePath.TrimStart('_');
                isUpdated = true;
            }
        }

        public override void WriteToFile(string filePath)
        {
            if (!isUpdated)
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(XmlStaticWebAssetsRoot));
            using var writer = new StreamWriter(filePath);
            using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            });
            serializer.Serialize(xmlWriter, staticWebAssetsRoot);
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
                var contentRelativeDirectory = basePath + Path.GetDirectoryName(contentFile)!.Replace(contentRootPath, string.Empty);
                AddOutput(contentFile, contentRelativeDirectory);
            }
        }
    }
}
