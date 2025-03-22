using System.Collections.Generic;

namespace Blazor.BrowserExtension.Analyzer.Translation
{
    internal static class SystemTranslations
    {
        private static Dictionary<string, string> systemTranslations = new()
        {
            { "System.Console", "console" },
            { "System.Console.WriteLine", "log" },
            { "System.Console.Write", "log" },
            { "Microsoft.Extensions.Logging.ILogger", "console" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogTrace", "trace" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogDebug", "debug" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogInformation", "log" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogWarning", "warn" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogError", "error" },
            { "Microsoft.Extensions.Logging.LoggerExtensions.LogCritical", "error" },
        };

        public static bool TryGetValue(string key, out string translation) => systemTranslations.TryGetValue(key, out translation);
    }
}
