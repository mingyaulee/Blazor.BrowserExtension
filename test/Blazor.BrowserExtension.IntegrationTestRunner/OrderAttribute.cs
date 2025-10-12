using System;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OrderAttribute(int order) : Attribute
    {
        public int Order { get; } = order;
    }
}
