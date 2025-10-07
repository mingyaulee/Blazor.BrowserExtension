using System.Reflection;
using System.Runtime.Versioning;

namespace Blazor.BrowserExtension.Analyzer.Test.Helpers
{
    public static class CommonTestHelper
    {
        public static string TargetFramework => targetFramework;
        public static string TargetFrameworkMajorVersion => targetFramework.Split('.')[0][3..];

        private static readonly string targetFramework = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkDisplayName!.ToLower().Replace(" ", string.Empty).TrimStart('.');
    }
}
