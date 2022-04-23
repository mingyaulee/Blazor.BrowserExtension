using System.IO;

namespace Blazor.BrowserExtension.IntegrationTestRunner
{
    public class IntegrationTestManifestV3 : BaseIntegrationTest
    {
        public IntegrationTestManifestV3(Fixture fixture) : base(fixture)
        {
            fixture.Initialize();
        }

        protected override void SetupBeforeInitialize()
        {
            var extensionPath = Fixture.GetExtensionPath();
            File.Copy(Path.Combine(extensionPath, "manifestv3.json"), Path.Combine(extensionPath, "manifest.json"), true);
        }
    }
}
