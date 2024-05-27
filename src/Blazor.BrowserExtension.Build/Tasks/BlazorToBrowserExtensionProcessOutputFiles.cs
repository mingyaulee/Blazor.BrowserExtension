using Blazor.BrowserExtension.Build.Tasks.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionProcessOutputFiles : Task
    {
        [Required]
        public ITaskItem[] Input { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var output = new List<ITaskItem>();
            foreach (var input in Input)
            {
                ProcessItem(input, output);
            }
            Output = output.ToArray();
            return true;
        }

        private static void ProcessItem(ITaskItem input, List<ITaskItem> output)
        {
            var itemPath = input.ItemSpec;
            var relativePath = OutputHelper.GetOutputRelativePath(GetRelativePath(input));
            output.Add(new TaskItem(itemPath, new Dictionary<string, string>()
            {
                { "ContentRelativeDirectory", relativePath }
            }));
        }

        private static string GetRelativePath(ITaskItem item)
        {
            return Path.GetDirectoryName(item.GetMetadata("RelativePath")) ?? string.Empty;
        }
    }
}
