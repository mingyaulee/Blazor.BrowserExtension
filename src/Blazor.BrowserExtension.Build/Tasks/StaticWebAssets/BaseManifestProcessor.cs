using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public abstract class BaseManifestProcessor
    {
        private readonly IEnumerable<string> excludePaths;
        private readonly List<StaticWebAssetFile> output;

        protected BaseManifestProcessor(IEnumerable<string> excludePaths)
        {
            this.excludePaths = excludePaths?.Select(excludePath => NormalizePath(excludePath)).ToList() ?? Enumerable.Empty<string>();
            output = new List<StaticWebAssetFile>();
        }

        public abstract void ReadFromFile(string filePath);

        public abstract void Process(string outputPath);

        public abstract void WriteToFile(string filePath);

        protected void AddOutput(string filePath, string relativePath)
        {
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar, '_').TrimEnd(Path.DirectorySeparatorChar);
            output.Add(new StaticWebAssetFile()
            {
                FilePath = filePath,
                RelativePath = relativePath
            });
        }

        public IEnumerable<StaticWebAssetFile> GetOutput()
        {
            return output;
        }

        protected bool ShouldExcludePath(string path)
        {
            return excludePaths.Any(excludePath => path.Equals(excludePath, StringComparison.OrdinalIgnoreCase));
        }

        protected static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
