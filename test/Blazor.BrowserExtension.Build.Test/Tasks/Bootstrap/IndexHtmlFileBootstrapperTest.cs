using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    public class IndexHtmlFileBootstrapperTest : BaseFileBootstrapperTest<IndexHtmlFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            <body>
                <div id="app">
                </div>
                <script src="_framework/blazor.webassembly.js"></script>
            </body>
            """;

        protected override string BootstrappedFileContent => """
            <body>
                <div id="app">
                </div>
                <script src="framework/blazor.webassembly.js"></script>
            </body>
            """;
    }
}
