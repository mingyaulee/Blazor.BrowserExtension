namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    [TestClass]
    public class ArrayTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            var items1 = new string[]
            {
                "Item1",
                "Item2",
                "Item3"
            };
            string[] items2 = new[]
            {
                "Item4"
            };
            object[] items3 =
            [
                true,
                123,
                123.45,
                123.45m
            ];
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            let items1 = [
              "Item1",
              "Item2",
              "Item3"
            ];
            let items2 = [
              "Item4"
            ];
            let items3 = [
              true,
              123,
              123.45,
              123.45
            ];
            """;
    }
}
