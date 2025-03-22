namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    public class DelegateTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            Console.WriteLine(OnInstalled);
            WebExtensions.Runtime.OnInstalled.AddListener(OnInstalled);
            """;

        protected override string BackgroundClassMembers =>
            """
            Task OnInstalled() => throw new NotImplementedException();
            """;

        protected override string ExpectedJsInstance =>
            """
            { "OnInstalled", (object)OnInstalled },
            { "OnInstalled0", (System.Delegate)OnInstalled },
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            console.log(fromReference('OnInstalled'));
            browser.runtime.onInstalled.addListener(fromReference('OnInstalled0'));
            """;
    }
}
