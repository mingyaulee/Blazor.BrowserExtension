using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionValidateManifest : Task
    {
        [Required]
        public string FileName { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.Normal, $"Reading content of file '{FileName}'");
                var fileLines = File.ReadAllLines(FileName);

                if (!ValidateManifestFile(fileLines))
                {
                    return false;
                }

                Log.LogMessage(MessageImportance.Normal, $"Validation completed for manifest file '{FileName}'");

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"An unexpected error occured when validating manifest file '{FileName}'");
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
            if (TryParseManifestFile(fileLines, out var manifestItems))
            {
                if (manifestItems.TryGetValue(ManifestItemKey.ManifestVersion, out var manifestVersion))
                {
                    if (manifestVersion.Value.Contains("2"))
                    {
                        return ValidateManifestV2Items(manifestItems);
                    }
                    else
                    {
                        LogErrorAtLine(0, "Only manifest version 2 is supported");
                        return false;
                    }
                }
                else
                {
                    LogErrorAtLine(0, "Manifest version 2 is not defined");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool TryParseManifestFile(string[] fileLines, out Dictionary<ManifestItemKey, ManifestItem> manifestItems)
        {
            var items = new Dictionary<ManifestItemKey, ManifestItem>();
            var invalidMatch = "(\\{|\\}|\\]).*(\\{|\\}|\\[|\\])";
            var keyMatch = "^\\s*\"(\\w+)\"\\s*:";
            ManifestItemKey? currentItem = null;
            var openingBracket = 0;
            var lineNumber = 0;
            var validationError = false;
            foreach (var line in fileLines)
            {
                lineNumber++;
                if (Regex.IsMatch(line, invalidMatch))
                {
                    validationError = true;
                    LogErrorAtLine(lineNumber, "The file manifest.json is not well formatted. Each line can only contain one of the characters '{', '}', '[', ']'");
                }
                var lineValue = line;
                if (openingBracket == 1)
                {
                    var match = Regex.Match(line, keyMatch);
                    if (match.Success)
                    {
                        currentItem = ParseManifestItemKey(match.Groups[1].Value);
                        lineValue = lineValue.Substring(lineValue.IndexOf(":") + 1);
                        if (currentItem.HasValue && !items.ContainsKey(currentItem.Value))
                        {
                            items[currentItem.Value] = new ManifestItem()
                            {
                                LineNumber = lineNumber,
                                Value = string.Empty
                            };
                        }
                    }
                }
                if (line.Contains("{"))
                {
                    openingBracket += line.Count(c => c == '{');
                }
                if (currentItem.HasValue && openingBracket >= 1 && !(openingBracket == 1 && line.Contains("}")))
                {
                    items[currentItem.Value].Value += lineValue;
                }
                if (line.Contains("}"))
                {
                    openingBracket -= line.Count(c => c == '}');
                }
            }

            manifestItems = items;
            return !validationError;
        }

        private bool ValidateManifestV2Items(Dictionary<ManifestItemKey, ManifestItem> manifestItems)
        {
            var validationError = false;
            foreach (var item in manifestItems)
            {
                var manifestItem = item.Value;
                switch (item.Key)
                {
                    case ManifestItemKey.Background:
                        if (!manifestItem.ContainsExactIgnoreCase("index.html?path=background"))
                        {
                            validationError = true;
                            LogErrorAtLine(manifestItem.LineNumber, "Manifest item 'background' must specify \"index.html?path=background\" as background page");
                        }
                        break;
                    case ManifestItemKey.WebAccessibleResources:
                        if (!manifestItem.ContainsExactIgnoreCase("framework/*")
                            || !manifestItem.ContainsExactIgnoreCase("BrowserExtensionScripts/*")
                            || !manifestItem.ContainsExactIgnoreCase("WebExtensionScripts/*"))
                        {
                            validationError = true;
                            LogErrorAtLine(manifestItem.LineNumber, "Manifest item 'web_accessible_resources' must specify \"framework/*\" and \"BrowserExtensionScripts/*\"");
                        }
                        break;
                    case ManifestItemKey.Permissions:
                        if (!manifestItem.ContainsExact("webRequest") || !manifestItem.ContainsExact("webRequestBlocking"))
                        {
                            validationError = true;
                            LogErrorAtLine(manifestItem.LineNumber, "Manifest item 'permissions' must specify \"webRequest\" and \"webRequestBlocking\"");
                        }
                        break;
                }
            }
            return !validationError;
        }

        private ManifestItemKey? ParseManifestItemKey(string key)
        {
            return key switch
            {
                "manifest_version" => ManifestItemKey.ManifestVersion,
                "options_page" => ManifestItemKey.OptionsPage,
                "browser_action" => ManifestItemKey.BrowserAction,
                "background" => ManifestItemKey.Background,
                "content_security_policy" => ManifestItemKey.ContentSecurityPolicy,
                "content_scripts" => ManifestItemKey.ContentScripts,
                "web_accessible_resources" => ManifestItemKey.WebAccessibleResources,
                "permissions" => ManifestItemKey.Permissions,
                _ => null,
            };
        }

        private enum ManifestItemKey
        {
            ManifestVersion,
            OptionsPage,
            BrowserAction,
            Background,
            ContentSecurityPolicy,
            ContentScripts,
            WebAccessibleResources,
            Permissions
        }

        private class ManifestItem
        {
            public int LineNumber { get; set; }
            public string Value { get; set; }

            public bool ContainsExact(string value)
            {
                return Regex.IsMatch(Value, Regex.Escape($"\"{value}\""));
            }

            public bool ContainsExactIgnoreCase(string value)
            {
                return Regex.IsMatch(Value, Regex.Escape($"\"{value}\""), RegexOptions.IgnoreCase);
            }
        }
    }
}
