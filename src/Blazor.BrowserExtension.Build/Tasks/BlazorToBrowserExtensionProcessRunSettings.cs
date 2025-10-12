using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public class BlazorToBrowserExtensionProcessRunSettings : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public string FilePath { get; set; }

        [Output]
        public string ApplicationUrl { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Processing launch settings '{FilePath}'");

                var fileContent = File.ReadAllText(FilePath);
                var json = JsonSerializer.Deserialize<JsonElement>(fileContent);

                if (json.TryGetProperty("profiles", out var profilesElement) && profilesElement.ValueKind == JsonValueKind.Object)
                {
                    ApplicationUrl = GetApplicationUrlFromProfiles(profilesElement);
                }
                else
                {
                    Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}No profiles found in launch settings");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"{LogPrefix}An unexpected error occurred when processing launch settings file '{FilePath}'");
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private string GetApplicationUrlFromProfiles(JsonElement profilesElement)
        {
            foreach (var profile in profilesElement.EnumerateObject().Where(IsProjectCommand))
            {
                if (profile.Value.TryGetProperty("applicationUrl", out var applicationUrlElement))
                {
                    Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Using default launch profile '{profile.Name}'");
                    return applicationUrlElement.GetString()?.Split(';')[0];
                }
                else
                {
                    Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}No applicationUrl found in launch profile '{profile.Name}'");
                }
            }

            return null;
        }

        private static bool IsProjectCommand(JsonProperty profile)
            => profile.Value.TryGetProperty("commandName", out var commandNameElement) && commandNameElement.ValueEquals("Project");
    }
}
