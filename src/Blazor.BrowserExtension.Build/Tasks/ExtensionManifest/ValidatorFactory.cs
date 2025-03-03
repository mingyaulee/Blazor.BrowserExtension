namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal static class ValidatorFactory
    {
        public static IValidator GetValidator(ManifestItem version)
        {
#if NETFRAMEWORK
            if (version.Value.Contains("3"))
#else
            if (version.Value.Contains('3'))
#endif
            {
                return new ManifestV3Validator();
            }

            return null;
        }
    }
}
