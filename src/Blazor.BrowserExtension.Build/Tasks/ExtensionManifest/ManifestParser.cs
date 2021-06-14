using System.Linq;
using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal static class ManifestParser
    {
        private const string INVALID_MATCH = "(\\{|\\}|\\]).*(\\{|\\}|\\[|\\])";
        private const string KEY_MATCH = "^\\s*\"(\\w+)\"\\s*:";
        public static bool TryParseManifestFile(string[] fileLines, out Manifest manifest)
        {
            var state = new ManifestParseState();
            foreach (var line in fileLines)
            {
                state.LineNumber++;
                if (IsInvalidMatch(line))
                {
                    state.AddError("The file manifest.json is not well formatted. Each line can only contain one of the characters '{', '}', '[', ']'");
                }
                var lineValue = line;
                if (state.IsAtRoot && TryParseManifestItemKey(line, out var manifestItemKey))
                {
                    lineValue = lineValue.Substring(lineValue.IndexOf(':') + 1);
                    state.AddManifestItem(manifestItemKey);
                }
                state.OpeningBracket += line.Count(c => c == '{');
                if (state.OpeningBracket >= 1)
                {
                    state.AddManifestItemValue(lineValue);
                }
                state.OpeningBracket -= line.Count(c => c == '}');
            }

            manifest = new Manifest()
            {
                Items = state.ManifestItems,
                ParseErrors = state.Errors
            };
            return !state.HasError;
        }

        private static bool IsInvalidMatch(string line)
        {
            return Regex.IsMatch(line, INVALID_MATCH);
        }

        private static bool TryParseManifestItemKey(string line, out ManifestItemKey? manifestItemKey)
        {
            var match = Regex.Match(line, KEY_MATCH);
            if (match.Success)
            {
                manifestItemKey = MapManifestItemKey(match.Groups[1].Value);
                return true;
            }
            manifestItemKey = null;
            return false;
        }

        private static ManifestItemKey? MapManifestItemKey(string key)
        {
            return key switch
            {
                "manifest_version" => ManifestItemKey.ManifestVersion,
                "options_ui" => ManifestItemKey.OptionsUi,
                "browser_action" => ManifestItemKey.BrowserAction,
                "background" => ManifestItemKey.Background,
                "content_security_policy" => ManifestItemKey.ContentSecurityPolicy,
                "content_scripts" => ManifestItemKey.ContentScripts,
                "web_accessible_resources" => ManifestItemKey.WebAccessibleResources,
                "permissions" => ManifestItemKey.Permissions,
                _ => null,
            };
        }
    }
}
