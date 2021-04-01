using System;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int order)
        {
            Order = order;
        }
        public int Order { get; }
    }
}
