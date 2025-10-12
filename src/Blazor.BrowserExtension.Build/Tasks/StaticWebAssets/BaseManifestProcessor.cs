using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public abstract class BaseManifestProcessor(IEnumerable<string> excludePaths)
    {
        private readonly IEnumerable<string> excludePaths = excludePaths?.Select(NormalizePath).ToList() ?? Enumerable.Empty<string>();
        private readonly List<StaticWebAssetFile> output = [];

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

        public IEnumerable<StaticWebAssetFile> GetOutput() => output;

        protected bool ShouldExcludePath(string path)
            => excludePaths.Any(excludePath => path.Equals(excludePath, StringComparison.OrdinalIgnoreCase));

        protected static string NormalizePath(string path)
            => Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
