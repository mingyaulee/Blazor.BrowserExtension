using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    public class ProgramCsFileBootstrapperTest : BaseFileBootstrapperTest<ProgramCsFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            using System;

            public static class Program
            {
                public static async Task Main(string[] args)
                {
                    var builder = WebAssemblyHostBuilder.CreateDefault(args);
                    builder.RootComponents.Add<App>("#app");
                    builder.RootComponents.Add<HeadOutlet>("head::after");

                    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
                    await builder.Build().RunAsync();
                }
            }
            """;

        protected override string BootstrappedFileContent => """
            using Blazor.BrowserExtension;
            using System;

            public static class Program
            {
                public static async Task Main(string[] args)
                {
                    var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
                    builder.UseBrowserExtension(browserExtension =>
                    {
                        if (browserExtension.Mode == BrowserExtensionMode.Background)
                        {
                            builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();
                        }
                        else
                        {
                            builder.RootComponents.Add<App>("#app");
                            builder.RootComponents.Add<HeadOutlet>("head::after");
                        }
                    });
            
                    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
                    await builder.Build().RunAsync();
                }
            }
            """;
    }
}
