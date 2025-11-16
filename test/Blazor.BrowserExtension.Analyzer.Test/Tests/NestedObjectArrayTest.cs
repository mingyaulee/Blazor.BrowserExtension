namespace Blazor.BrowserExtension.Analyzer.Test.Tests
{
    [TestClass]
    public class NestedObjectArrayTest : BaseBackgroundSourceGeneratorTest
    {
        protected override string MainMethodBody =>
            """
            var nestedObject = new
            {
                NestedObject1 = new
                {
                    StringProperty = "Test",
                    BoolProperty = true
                },
                NestedObject2 = new {
                    IntProperty = 123,
                    DoubleProperty = 123.45,
                    DecimalProperty = 123.45m
                },
                NestedArray = new object[]
                {
                    "StringItem",
                    true
                }
            };
            var nestedArray = new object[]
            {
                "Item1",
                new
                {
                    Property = "Value"
                }
            };
            """;

        protected override string ExpectedBackgroundWorkerJs =>
            """
            let nestedObject = {
              NestedObject1: {
                StringProperty: "Test",
                BoolProperty: true
              },
              NestedObject2: {
                IntProperty: 123,
                DoubleProperty: 123.45,
                DecimalProperty: 123.45
              },
              NestedArray: [
                "StringItem",
                true
              ]
            };
            let nestedArray = [
              "Item1",
              {
                Property: "Value"
              }
            ];
            """;
    }
}
