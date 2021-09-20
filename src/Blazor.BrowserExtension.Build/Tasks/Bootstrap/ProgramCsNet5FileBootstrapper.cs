using System;
using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public class ProgramCsNet5FileBootstrapper : IFileBootstrapper
    {
        public bool Bootstrap(List<string> fileLines)
        {
            var isUpdated = false;

            // Replace
            // builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            // with
            // builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            var registerHttpClientIndex = fileLines.FindIndex(fileLine => fileLine.Contains("HttpClient"));
            if (registerHttpClientIndex > -1 && !fileLines[registerHttpClientIndex].Contains("JsHttpClient"))
            {
                fileLines[registerHttpClientIndex] = "            builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });";
                isUpdated = true;
            }

            // Insert
            // builder.Services.AddBrowserExtensionServices(options =>
            // {
            //     options.ProjectNamespace = typeof(Program).Namespace;
            // });
            // before await builder.Build().RunAsync();
            var registerServicesIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".AddBrowserExtensionServices"));
            if (registerServicesIndex == -1)
            {
                var buildHostIndex = fileLines.FindIndex(fileLine => fileLine.Contains(".Build().RunAsync()"));
                if (buildHostIndex == -1)
                {
                    throw new InvalidOperationException("Unable to find builder.Build().RunAsync() in Program.cs file.");
                }
                fileLines.Insert(buildHostIndex + 0, "            builder.Services.AddBrowserExtensionServices(options =>");
                fileLines.Insert(buildHostIndex + 1, "            {");
                fileLines.Insert(buildHostIndex + 2, "                options.ProjectNamespace = typeof(Program).Namespace;");
                fileLines.Insert(buildHostIndex + 3, "            });");
                fileLines.Insert(buildHostIndex + 4, "");
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
