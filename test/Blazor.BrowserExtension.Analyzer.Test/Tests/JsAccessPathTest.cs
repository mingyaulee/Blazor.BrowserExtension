﻿namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    public class JsAccessPathTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            Console.WriteLine(WebExtensions.Runtime.OnInstalled);
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            console.log(browser.runtime.onInstalled);
            """;
    }
}
