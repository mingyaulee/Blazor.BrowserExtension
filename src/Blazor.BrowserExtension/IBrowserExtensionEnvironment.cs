namespace Blazor.BrowserExtension
{
    public interface IBrowserExtensionEnvironment
    {
        BrowserExtensionMode Mode { get; }
        static IBrowserExtensionEnvironment Instance { get; internal set; }
    }
}
