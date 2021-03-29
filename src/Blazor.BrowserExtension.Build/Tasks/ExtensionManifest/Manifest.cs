using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class Manifest
    {
        public IDictionary<ManifestItemKey, ManifestItem> Items { get; set; }
        public IEnumerable<ManifestParseError> ParseErrors { get; set; }
    }
}
