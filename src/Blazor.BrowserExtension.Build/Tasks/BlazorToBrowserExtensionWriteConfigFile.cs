using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionWriteConfigFile : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public ITaskItem[] Input { get; set; }

        [Required]
        public string FilePath { get; set; }

        public override bool Execute()
        {
            try
            {
                var option = GetOption(Input);
                var optionJson = JsonSerializer.Serialize(option);

                var directory = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(FilePath, optionJson);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"{LogPrefix}An unexpected error occurred when writing option file '{FilePath}'");
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private static Dictionary<string, object> GetOption(IEnumerable<ITaskItem> items)
        {
            var option = new Dictionary<string, object>();
            foreach (var item in items)
            {
                var key = item.GetMetadata("Key");
                var valueString = item.GetMetadata("Value");
                var type = item.GetMetadata("Type");
                option[key] = GetValue(valueString, type);
            }
            return option;
        }

        private static object GetValue(string value, string type)
        {
            if (type == "bool")
            {
                return bool.TryParse(value, out var boolValue) && boolValue;
            }

            return value;
        }
    }
}
