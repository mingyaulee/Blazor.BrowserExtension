using System.IO;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class IntegrationTestManifestV3 : BaseIntegrationTest
    {
        protected override void SetupBeforeInitialize()
            => File.Copy(Path.Combine(extensionPath, "manifestv3.json"), Path.Combine(extensionPath, "manifest.json"), true);
    }
}
