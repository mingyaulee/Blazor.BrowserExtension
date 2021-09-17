#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public class JsonManifestParser : BaseManifestParser
    {
        private List<string> contentRoots = new();

        public JsonManifestParser(IEnumerable<string> excludePaths) : base(excludePaths)
        {
        }

        public override void ReadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var staticWebAssetManifest = StaticWebAssetManifest.Parse(json);
            if (staticWebAssetManifest?.ContentRoots is null || !staticWebAssetManifest.ContentRoots.Any())
            {
                return;
            }

            contentRoots = staticWebAssetManifest.ContentRoots.ToList();
            VisitNode(staticWebAssetManifest.Root, "/");
        }

        private void VisitNode(StaticWebAssetNode content, string contentRelativePath)
        {
            if (content is null)
            {
                return;
            }

            if (content.HasChildren())
            {
                foreach (var child in content.Children!)
                {
                    VisitNode(child.Value, $"{contentRelativePath.TrimEnd('/')}/{child.Key}");
                }
            }

            if (content.HasPatterns())
            {
                ProcessPatterns(content.Patterns!, contentRelativePath);
            }

            if (content.Match is not null)
            {
                ProcessMatch(content.Match!, contentRelativePath);
            }
        }

        private void ProcessPatterns(IEnumerable<StaticWebAssetPattern> patterns, string contentRelativePath)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.Pattern == "**")
                {
                    var contentRootPath = NormalizePath(contentRoots[pattern.ContentRoot]);
                    if (ShouldExcludePath(contentRootPath))
                    {
                        continue;
                    }

                    var contentFiles = Directory.GetFiles(contentRootPath, "*", SearchOption.AllDirectories);
                    foreach (var contentFile in contentFiles)
                    {
                        var contentRelativeDirectory = contentRelativePath + Path.GetDirectoryName(contentFile)?.Replace(contentRootPath, string.Empty);
                        AddOutput(contentFile, contentRelativeDirectory);
                    }
                }
                else
                {
                    throw new NotSupportedException($"Static web assets manifest pattern '{pattern.Pattern}' is not supported.");
                }
            }
        }

        private void ProcessMatch(StaticWebAssetMatch match, string contentRelativePath)
        {
            var contentRootPath = NormalizePath(contentRoots[match.ContentRoot]);
            if (ShouldExcludePath(contentRootPath))
            {
                return;
            }

            if (match.Path.Replace('\\', '/').StartsWith(@"_framework/dotnet.", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contentFile = Path.Combine(contentRootPath, match.Path);
            var contentRelativeDirectory = Path.GetDirectoryName(contentRelativePath);
            AddOutput(contentFile, contentRelativeDirectory);
        }
    }
}
