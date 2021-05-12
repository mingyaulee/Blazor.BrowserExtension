using System;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public static class BootstrapperFactory
    {
        public static IFileBootstrapper GetBootstrapper(BootstrapFileType bootstrapFileType)
        {
            return bootstrapFileType switch
            {
                BootstrapFileType.Project => new ProjectFileBootstrapper(),
                BootstrapFileType.IndexHtml => new IndexHtmlFileBootstrapper(),
                BootstrapFileType.IndexRazor => new IndexRazorFileBootstrapper(),
                BootstrapFileType.ProgramCs => new ProgramCsFileBootstrapper(),
                _ => throw new InvalidOperationException($"Bootstrap file type '{bootstrapFileType}' is not supported.")
            };
        }
    }
}
