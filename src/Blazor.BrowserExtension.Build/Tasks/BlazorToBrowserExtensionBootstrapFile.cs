using Blazor.BrowserExtension.Build.Tasks.Bootstrap;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionBootstrapFile : Task
    {
        [Required]
        public string FilePath { get; set; }

        [Required]
        public string FileType { get; set; }

        public override bool Execute()
        {
            try
            {
                var bootstrapFileType = ParseFileType(FileType);

                Log.LogMessage(MessageImportance.Normal, $"Reading content of {bootstrapFileType} file '{FilePath}'");
                var fileLines = File.ReadAllLines(FilePath).ToList();
                var bootstrapper = BootstrapperFactory.GetBootstrapper(bootstrapFileType);

                if (bootstrapper.Bootstrap(fileLines))
                {
                    File.WriteAllLines(FilePath, fileLines);
                    Log.LogMessage(MessageImportance.Normal, $"Bootstrapping completed for {bootstrapFileType} file '{FilePath}'");
                }
                else
                {
                    Log.LogMessage(MessageImportance.Normal, $"Bootstrapping skipped for {bootstrapFileType} file '{FilePath}'");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"An unexpected error occured when bootstrapping file '{FilePath}'");
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private BootstrapFileType ParseFileType(string fileType)
        {
            if (Enum.TryParse<BootstrapFileType>(fileType, out var bootstrapFileType))
            {
                return bootstrapFileType;
            }

#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            throw new ArgumentOutOfRangeException(nameof(FileType), $"{nameof(FileType)} is not a recognized {nameof(BootstrapFileType)}.");
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
        }
    }
}
