using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestV2Validator : IValidator
    {
        public IEnumerable<ValidationResult> Validate(IDictionary<ManifestItemKey, ManifestItem> manifestItems)
        {
            var validationResults = new List<ValidationResult>();
            foreach (var item in manifestItems)
            {
                var manifestItem = item.Value;
                switch (item.Key)
                {
                    case ManifestItemKey.Background:
                        if (!manifestItem.ContainsExactIgnoreCase("index.html?path=background"))
                        {
                            validationResults.Add(new ValidationResult()
                            {
                                Item = manifestItem,
                                Error = "Manifest item 'background' must specify \"index.html?path=background\" as background page"
                            });
                        }
                        break;
                    case ManifestItemKey.WebAccessibleResources:
                        if (!manifestItem.ContainsExactIgnoreCase("framework/*")
                            || !manifestItem.ContainsExactIgnoreCase("BrowserExtensionScripts/*")
                            || !manifestItem.ContainsExactIgnoreCase("WebExtensionScripts/*"))
                        {
                            validationResults.Add(new ValidationResult()
                            {
                                Item = manifestItem,
                                Error = "Manifest item 'web_accessible_resources' must specify \"framework/*\" and \"BrowserExtensionScripts/*\""
                            });
                        }
                        break;
                    case ManifestItemKey.Permissions:
                        if (!manifestItem.ContainsExact("webRequest") || !manifestItem.ContainsExact("webRequestBlocking"))
                        {
                            validationResults.Add(new ValidationResult()
                            {
                                Item = manifestItem,
                                Error = "Manifest item 'permissions' must specify \"webRequest\" and \"webRequestBlocking\""
                            });
                        }
                        break;
                }
            }
            return validationResults;
        }
    }
}
