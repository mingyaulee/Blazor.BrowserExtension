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
                    case ManifestItemKey.WebAccessibleResources:
                        if (!manifestItem.ContainsExactIgnoreCase("framework/*")
                            || !manifestItem.ContainsExactIgnoreCase("content/*"))
                        {
                            validationResults.Add(new ValidationResult()
                            {
                                Item = manifestItem,
                                Error = "Manifest item 'web_accessible_resources' must specify \"framework/*\" and \"content/*\"."
                            });
                        }
                        break;
                }
            }
            return validationResults;
        }
    }
}
