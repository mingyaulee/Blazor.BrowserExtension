using Blazor.BrowserExtension.Build.Tasks.ExtensionManifest;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionValidateManifest : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public string FileName { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Reading content of file '{FileName}'");
                var fileLines = File.ReadAllLines(FileName);

                if (!ValidateManifestFile(fileLines))
                {
                    return false;
                }

                Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Validation completed for manifest file '{FileName}'");

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"{LogPrefix}An unexpected error occured when validating manifest file '{FileName}'");
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void LogErrorAtLine(int lineNumber, string message)
        {
            Log.LogError(null, null, null, FileName, lineNumber, 0, lineNumber, 0, message);
        }

        private bool ValidateManifestFile(string[] fileLines)
        {
            var isValid = ManifestParser.TryParseManifestFile(fileLines, out var manifest);
            if (isValid)
            {
                return ValidateManifestItems(manifest.Items);
            }
            else
            {
                foreach (var error in manifest.ParseErrors)
                {
                    LogErrorAtLine(error.LineNumber, error.Message);
                }
                return false;
            }
        }

        private bool ValidateManifestItems(IDictionary<ManifestItemKey, ManifestItem> manifestItems)
        {
            if (manifestItems.TryGetValue(ManifestItemKey.ManifestVersion, out var manifestVersion))
            {
                var validator = ValidatorFactory.GetValidator(manifestVersion);
                if (validator is null)
                {
                    LogErrorAtLine(manifestVersion.LineNumber, $"Manifest version '{manifestVersion.Value}' is not supported");
                    return false;
                }

                var validationResults = validator.Validate(manifestItems);
                if (validationResults.Any())
                {
                    foreach (var validationResult in validationResults)
                    {
                        LogErrorAtLine(validationResult.Item.LineNumber, validationResult.Error);
                    }
                    return false;
                }
                return true;
            }
            else
            {
                LogErrorAtLine(0, "Manifest version is not defined");
                return false;
            }
        }
    }
}
