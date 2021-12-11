namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal static class ValidatorFactory
    {
        public static IValidator GetValidator(ManifestItem version)
        {
#if NETFRAMEWORK
            if (version.Value.Contains("2"))
#else
            if (version.Value.Contains('2'))
#endif
            {
                return new ManifestV2Validator();
            }
            return null;
        }
    }
}
