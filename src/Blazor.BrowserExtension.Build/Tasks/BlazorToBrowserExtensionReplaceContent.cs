using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionReplaceContent : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public ITaskItem[] Files { get; set; }

        [Required]
        public ITaskItem[] Replace { get; set; }

        public bool FailOnWarning { get; set; }

        public string FailMessage { get; set; }

        private string FileName { get; set; }

        public override bool Execute()
        {
            try
            {
                var hasWarning = false;
                foreach (var file in Files)
                {
                    FileName = file.ItemSpec;

                    Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Replacing content of file '{FileName}'");
                    var fileContent = File.ReadAllText(FileName);
                    var fileChanged = false;

                    foreach (var replace in Replace)
                    {
                        var from = replace.GetMetadata("From");
                        var to = replace.GetMetadata("To");

                        Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Replacing {replace.ItemSpec}");
                        if (fileContent.Contains(from))
                        {
                            fileContent = fileContent.Replace(from, to);
                            fileChanged = true;
                            Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Replaced from '{from}' to '{to}'");
                        }
                        else
                        {
                            Log.LogWarning($"{LogPrefix}Unable to find text to replace '{from}' in file '{FileName}'");
                            hasWarning = true;
                        }
                    }

                    if (fileChanged)
                    {
                        File.WriteAllText(FileName, fileContent);
                        Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Replace content completed of file '{FileName}'");
                    }
                }

                if (FailOnWarning && hasWarning)
                {
                    if (!string.IsNullOrEmpty(FailMessage))
                    {
                        Log.LogError(LogPrefix + FailMessage);
                    }
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"{LogPrefix}An unexpected error occured when replacing file content of file '{FileName}'");
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
