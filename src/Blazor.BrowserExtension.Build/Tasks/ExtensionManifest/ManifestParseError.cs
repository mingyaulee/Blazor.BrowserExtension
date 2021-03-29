namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestParseError
    {
        public int LineNumber { get; set; }
        public string Message { get; set; }
    }
}
