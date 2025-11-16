using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    [TestClass]
    public class ImportsRazorFileBootstrapperTest : BaseFileBootstrapperTest<ImportsRazorFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            @using Microsoft.AspNetCore.Components.Forms
            @using Microsoft.AspNetCore.Components.Routing
            @using Microsoft.AspNetCore.Components.Web
            
            """;

        protected override string BootstrappedFileContent => """
            @using Microsoft.AspNetCore.Components.Forms
            @using Microsoft.AspNetCore.Components.Routing
            @using Microsoft.AspNetCore.Components.Web
            @using Blazor.BrowserExtension.Pages
            
            """;
    }
}
