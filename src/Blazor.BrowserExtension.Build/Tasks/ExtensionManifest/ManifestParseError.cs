namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestParseError(long? lineNumber, long? columnNumber, string message)
    {
        public long? LineNumber { get; } = lineNumber;
        public long? ColumnNumber { get; } = columnNumber;
        public string Message { get; } = message;
    }
}
