namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal static class ValidatorFactory
    {
        public static IValidator GetValidator(ManifestItem version)
        {
            if (version.Value.Contains("2"))
            {
                return new ManifestV2Validator();
            }
            return null;
        }
    }
}
