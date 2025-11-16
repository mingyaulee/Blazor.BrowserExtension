namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    [TestClass]
    public class SystemTranslationTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            Console.WriteLine("WriteLine");
            Console.Write("Write");
            Logger.LogTrace("Trace");
            Logger.LogDebug("Debug");
            Logger.LogInformation("Information");
            Logger.LogWarning("Warning");
            Logger.LogError("Error");
            Logger.LogCritical("Critical");
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            console.log("WriteLine");
            console.log("Write");
            console.trace("Trace");
            console.debug("Debug");
            console.log("Information");
            console.warn("Warning");
            console.error("Error");
            console.error("Critical");
            """;
    }
}
