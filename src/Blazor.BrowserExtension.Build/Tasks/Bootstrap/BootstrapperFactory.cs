﻿using System;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public static class BootstrapperFactory
    {
        public static IFileBootstrapper GetBootstrapper(BootstrapFileType bootstrapFileType)
            => bootstrapFileType switch
            {
                BootstrapFileType.Project => new ProjectFileBootstrapper(),
                BootstrapFileType.IndexRazor => new IndexRazorFileBootstrapper(),
                BootstrapFileType.ImportsRazor => new ImportsRazorFileBootstrapper(),
                BootstrapFileType.ProgramCs => new ProgramCsFileBootstrapper(),
                _ => throw new InvalidOperationException($"Bootstrap file type '{bootstrapFileType}' is not supported.")
            };
    }
}
