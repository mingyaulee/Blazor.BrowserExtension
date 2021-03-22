using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension
{
    public class BrowserExtensionOption
    {
        public string ProjectNamespace { get; set; }

        public string GetSafeProjectNamespace()
        {
            return Regex.Replace(ProjectNamespace, "[\\.|\\s|\\-]", "_");
        }
    }
}
