using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionSafeNamespace : Task
    {
        [Required]
        public string Input { get; set; }

        public string Replacement { get; set; }

        [Output]
        public string Output { get; set; }

        public override bool Execute()
        {
            Output = Regex.Replace(Input, "[\\.|\\s|\\-]", Replacement ?? string.Empty);
            return true;
        }
    }
}
