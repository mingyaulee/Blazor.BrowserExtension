using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionProcessPublishFiles : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public ITaskItem[] Input { get; set; }
        public string CompressionEnabled { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var compressionEnabled = "true".Equals(CompressionEnabled, StringComparison.OrdinalIgnoreCase);
            var output = new List<ITaskItem>();
            foreach (var input in Input)
            {
                ProcessItem(input, output, compressionEnabled);
            }
            Output = output.ToArray();
            return true;
        }

        private void ProcessItem(ITaskItem input, List<ITaskItem> output, bool compressionEnabled)
        {
            var itemPath = input.ItemSpec;
            var relativePath = GetRelativePath(input);
            if (string.IsNullOrEmpty(relativePath))
            {
                return;
            }

            if (itemPath.EndsWith(".gz"))
            {
                LogExcludeItem(relativePath);
                output.Add(input);
            }
            else if (itemPath.EndsWith(".br"))
            {
                ProcessCompressedItem(relativePath, input, output, compressionEnabled);
            }
        }

        private void ProcessCompressedItem(string relativePath, ITaskItem input, List<ITaskItem> output, bool compressionEnabled)
        {
#if NET5_0_OR_GREATER
            var uncompressedItemRelativePath = relativePath[0..^3];
#else
            var uncompressedItemRelativePath = relativePath.Substring(0, relativePath.Length - 3);
#endif
            if (ShouldUseCompressedFile(uncompressedItemRelativePath))
            {
                if (compressionEnabled)
                {
                    var matchingFiles = Input.Where(item => uncompressedItemRelativePath.Equals(GetRelativePath(item), StringComparison.OrdinalIgnoreCase)).ToArray();
                    LogExcludeItems(matchingFiles);
                    output.AddRange(matchingFiles);
                }
                else
                {
                    output.Add(input);
                }
            }
            else
            {
                LogExcludeItem(relativePath);
                output.Add(input);
            }
        }

        private static bool ShouldUseCompressedFile(string filePath)
        {
            return filePath.Contains("framework") &&
                !(filePath.EndsWith(".js") || filePath.EndsWith(".json"));
        }

        private void LogExcludeItem(string relativePath)
        {
            Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Excluding {relativePath}");
        }

        private void LogExcludeItems(IEnumerable<ITaskItem> items)
        {
            foreach (var item in items)
            {
                LogExcludeItem(GetRelativePath(item));
            }
        }

        private static string GetRelativePath(ITaskItem item)
        {
            return item.GetMetadata("RelativePath");
        }
    }
}
