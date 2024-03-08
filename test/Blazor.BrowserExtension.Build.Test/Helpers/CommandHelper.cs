using System;
using System.Diagnostics;

namespace Blazor.BrowserExtension.Build.Test.Helpers
{
    public static class CommandHelper
    {
        public static string ExecuteCommand(string command, string arguments, string workingDirectory = null)
            => ExecuteCommandInternal(command, arguments, workingDirectory, true);

        public static void ExecuteCommandVoid(string command, string arguments, string workingDirectory = null)
            => ExecuteCommandInternal(command, arguments, workingDirectory, false);

        private static string ExecuteCommandInternal(string command, string arguments, string workingDirectory, bool readOutput)
        {
            var processStartInfo = new ProcessStartInfo(command, arguments)
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
            };
            
            var process = new Process()
            {
                StartInfo = processStartInfo
            };

            process.Start();
            process.WaitForExit(TimeSpan.FromMinutes(2));

            if (!process.HasExited || process.ExitCode != 0)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                var commandDetails = $"'{command}{arguments?.PadLeft(arguments.Length + 1)}'{(string.IsNullOrEmpty(workingDirectory) ? null : $" in '{workingDirectory}'")}";

                if (!process.HasExited)
                {
                    throw new Exception($"Command {commandDetails} timed out. Error output: '{error}'. Standard output: '{output}'");
                }
                else if (process.ExitCode != 0)
                {
                    throw new Exception($"Command {commandDetails} exited with code {process.ExitCode}. Error output: '{error}'. Standard output: '{output}'");
                }
            }

            if (readOutput)
            {
                return process.StandardOutput.ReadToEnd();
            }

            return null;
        }
    }
}
