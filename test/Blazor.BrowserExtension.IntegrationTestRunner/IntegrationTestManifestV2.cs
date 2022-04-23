using System.IO;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class IntegrationTestManifestV2 : BaseIntegrationTest
    {
        public IntegrationTestManifestV2(Fixture fixture) : base(fixture)
        {
            fixture.Initialize();
        }

        protected override void SetupBeforeInitialize()
        {
            var extensionPath = Fixture.GetExtensionPath();
            File.Copy(Path.Combine(extensionPath, "manifestv2.json"), Path.Combine(extensionPath, "manifest.json"), true);
        }
    }
}
