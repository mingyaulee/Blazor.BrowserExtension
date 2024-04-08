using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Helpers
{
    internal static class OutputHelper
    {
        public static IDictionary<string, string> PathMappings { get; } = new Dictionary<string, string>()
        {
            { "_framework", "framework" },
            { "_content", "content" }
        };

        public static string GetOutputRelativePath(string relativePath)
        {
            foreach (var mapping in PathMappings)
            {
                relativePath = relativePath?.Replace(mapping.Key, mapping.Value);
            }
            return relativePath;
        }
    }
}
