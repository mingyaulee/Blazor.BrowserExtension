namespace Blazor.BrowserExtension
{
    public class BrowserExtensionEnvironment : IBrowserExtensionEnvironment
    {
        public BrowserExtensionEnvironment(BrowserExtensionMode mode)
        {
            Mode = mode;
        }

        public BrowserExtensionMode Mode { get; }
    }
}
