using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal interface IValidator
    {
        IEnumerable<ValidationResult> Validate(IDictionary<ManifestItemKey, ManifestItem> manifestItems);
    }
}
