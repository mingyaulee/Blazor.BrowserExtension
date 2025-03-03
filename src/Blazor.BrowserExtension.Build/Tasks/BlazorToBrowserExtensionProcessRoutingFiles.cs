﻿using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blazor.BrowserExtension.Build.Tasks
{
    public partial class BlazorToBrowserExtensionProcessRoutingFiles : Task
    {
        private const string LogPrefix = "    ";

        [Required]
        public ITaskItem[] Input { get; set; }

        [Required]
        public string AssetsPath { get; set; }

        [Required]
        public string EntryFile { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var output = new List<ITaskItem>();
            var pageRoutes = new HashSet<string>();
            foreach (var input in Input)
            {
                var fullPath = input.GetMetadata("FullPath");
                var filePageRoutes = ReadDirectivesFromFile(fullPath)
                    .Where(line => line.Contains("@page "))
                    .Select(ParsePageRoute)
                    .Where(IsFileName)
                    .ToArray();
                foreach (var pageRoute in filePageRoutes)
                {
                    pageRoutes.Add(pageRoute);
                }
            }

            if (!File.Exists(EntryFile))
            {
                throw new InvalidOperationException($"Entry file does not exists '{EntryFile}'.");
            }

            foreach (var pageRoute in pageRoutes)
            {
                var routeHandlingFilePath = Path.Combine(AssetsPath, pageRoute);
                if (File.Exists(routeHandlingFilePath))
                {
                    Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Excluded route '{pageRoute}'");
                    continue;
                }

                var taskItem = new TaskItem(EntryFile);
                taskItem.SetMetadata("Route", pageRoute);
                output.Add(taskItem);
                Log.LogMessage(MessageImportance.Normal, $"{LogPrefix}Added route '{pageRoute}'");
            }

            Output = output.ToArray();
            return true;
        }

        private static IEnumerable<string> ReadDirectivesFromFile(string path)
        {
            var index = 0;
            foreach (var line in File.ReadLines(path))
            {
                if (++index == 10)
                {
                    break;
                }

                yield return line;
            }
        }


#if NET8_0_OR_GREATER
        [GeneratedRegex(@"@page\s+""(?'route'[^""]+)")]
        private static partial Regex GetRouteRegex();
#else
        private static Regex GetRouteRegex() => new(@"@page\s+""(?'route'[^""]+)");
#endif
        private static string ParsePageRoute(string pageDirective)
        {
            var match = GetRouteRegex().Match(pageDirective);
            if (match.Success && match.Groups["route"].Success)
            {
                return match.Groups["route"].Value.TrimStart('/');
            }

            return string.Empty;
        }

        private static bool IsFileName(string pageDirective)
        {
            return !string.IsNullOrEmpty(Path.GetExtension(pageDirective));
        }
    }
}
