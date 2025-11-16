using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    [TestClass]
    public class IndexRazorFileBootstrapperTest : BaseFileBootstrapperTest<IndexRazorFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            @page "/"

            <h1>Hello, from Blazor.</h1>
            """;

        protected override string BootstrappedFileContent => """
            @page "/index.html"
            @inherits IndexPage
            
            <h1>Hello, from Blazor.</h1>
            """;
    }
}
