using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal class ManifestParseState
    {
        private readonly List<ManifestParseError> errors;
        public ManifestParseState()
        {
            ManifestItems = new Dictionary<ManifestItemKey, ManifestItem>();
            Errors = errors = new List<ManifestParseError>();
        }

        public IDictionary<ManifestItemKey, ManifestItem> ManifestItems { get; }
        public IEnumerable<ManifestParseError> Errors { get; }
        public bool HasError => errors.Count > 0;
        public int OpeningBracket { get; set; }
        public bool IsAtRoot => OpeningBracket == 1;
        public int LineNumber { get; set; }
        public ManifestItemKey? CurrentItemKey { get; set; }

        public void AddManifestItem(ManifestItemKey? itemKey)
        {
            CurrentItemKey = itemKey;
            if (CurrentItemKey.HasValue && !ManifestItems.ContainsKey(CurrentItemKey.Value))
            {
                ManifestItems.Add(CurrentItemKey.Value, new ManifestItem(LineNumber));
            }
        }

        public void AddManifestItemValue(string value)
        {
#if NETFRAMEWORK
            if (CurrentItemKey.HasValue && !(IsAtRoot && value.Contains("}")))
#else
            if (CurrentItemKey.HasValue && !(IsAtRoot && value.Contains('}')))
#endif
            {
                ManifestItems[CurrentItemKey.Value].AddValue(value);
            }
        }

        public void AddError(string error)
        {
            errors.Add(new ManifestParseError()
            {
                LineNumber = LineNumber,
                Message = error
            });
        }
    }
}
