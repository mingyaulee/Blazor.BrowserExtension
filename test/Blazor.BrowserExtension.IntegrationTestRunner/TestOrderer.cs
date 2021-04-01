using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class TestOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return testCases.OrderBy(testCase =>
            {
                var attributeInfo = testCase.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                return attributeInfo?.GetNamedArgument<int>(nameof(OrderAttribute.Order)) ?? 0;
            });
        }
    }
}
