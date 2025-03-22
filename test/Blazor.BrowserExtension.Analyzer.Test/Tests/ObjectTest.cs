namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    public class ObjectTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            var object1 = new
            {
                StringProperty = "Test",
                BoolProperty = true,
                IntProperty = 123,
                DoubleProperty = 123.45,
                DecimalProperty = 123.45
            };
            var object2 = new WebExtensions.Net.WebRequest.RequestFilter()
            {
                Incognito = true
            };
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            let object1 = {
              StringProperty: "Test",
              BoolProperty: true,
              IntProperty: 123,
              DoubleProperty: 123.45,
              DecimalProperty: 123.45
            };
            let object2 = {
              incognito: true
            };
            """;
    }
}
