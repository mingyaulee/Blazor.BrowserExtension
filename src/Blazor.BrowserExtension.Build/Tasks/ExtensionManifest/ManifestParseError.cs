namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestParseError
    {
        public ManifestParseError(long? lineNumber, long? columnNumber, string message)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            Message = message;
        }

        public long? LineNumber { get; }
        public long? ColumnNumber { get; }
        public string Message { get; }
    }
}
