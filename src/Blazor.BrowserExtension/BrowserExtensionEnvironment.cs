namespace Blazor.BrowserExtension
{
    public class BrowserExtensionEnvironment : IBrowserExtensionEnvironment
    {
        public BrowserExtensionMode Mode { get; init; }
        public string BaseUrl { get; init; }
    }
}
