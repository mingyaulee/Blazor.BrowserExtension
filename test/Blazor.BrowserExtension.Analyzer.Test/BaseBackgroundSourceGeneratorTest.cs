using System.Reflection;
using Blazor.BrowserExtension.Analyzer.Test.Helpers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Blazor.BrowserExtension.Analyzer.Test
{
    public abstract class BaseBackgroundSourceGeneratorTest
    {
        [Fact]
        public async Task TestGenerateSource()
        {
            var test = new CSharpSourceGeneratorTest<BackgroundSourceGenerator, DefaultVerifier>();

            test.TestState.ReferenceAssemblies = GetReferenceAssemblies()
                .AddPackages([
                    new PackageIdentity("Microsoft.AspNetCore.Components.WebAssembly", $"{CommonTestHelper.TargetFrameworkMajorVersion}.0.0"),
                    new PackageIdentity("WebExtensions.Net.Extensions.DependencyInjection", "4.1.0")
                ]).AddAssemblies([
                    "Microsoft.AspNetCore.Components.WebAssembly",
                    "WebExtensions.Net.Extensions.DependencyInjection",
                    "Blazor.BrowserExtension",
                ]);

            test.TestState.AdditionalReferences.Add(typeof(BackgroundWorkerBase).Assembly);

            var testName = GetType().Name;
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var testDirectory = Path.Combine(currentDirectory, "TestOutput", testName);

            // Setup output directory for JS file generation from source generator
            var jsOutputDirectory = Path.Combine(testDirectory, "obj");
            if (Directory.Exists(jsOutputDirectory))
            {
                Directory.Delete(jsOutputDirectory, true);
            }
            Directory.CreateDirectory(jsOutputDirectory);
            await File.WriteAllTextAsync(Path.Combine(testDirectory, $"{testName}.csproj"), string.Empty);

            var backgroundWorkerFile = ReadFromEmbeddedResource("BackgroundWorkerTemplate.cs")
                .Replace("/* Main_Method_Body_Placeholder */", IndentContent(MainMethodBody, 12))
                .Replace("/* Members_Placeholder */", IndentContent(BackgroundClassMembers, 8));
            test.TestState.Sources.Add((Path.Combine(testDirectory, "BackgroundWorker.cs"), backgroundWorkerFile));

            var expectedBackgroundWorkerGeneratedFile = ReadFromEmbeddedResource("BackgroundWorkerGeneratedTemplate.cs")
                .Replace("/* JsInstance_Placeholder */", IndentContent(ExpectedJsInstance, 12))
                .TrimEnd();
            test.TestState.GeneratedSources.Add((typeof(BackgroundSourceGenerator), "BackgroundWorker.generated.cs", expectedBackgroundWorkerGeneratedFile));

            await test.RunAsync();

            var jsFile = Path.Combine(jsOutputDirectory, "BackgroundWorkerMain.generated.js");
            Assert.True(File.Exists(jsFile), $"File exists '{jsFile}'");
            Assert.Equal(ExpectedBackgroundWorkerJs, (await File.ReadAllTextAsync(jsFile)).TrimEnd());
        }

        protected abstract string MainMethodBody { get; }
        protected virtual string BackgroundClassMembers => string.Empty;
        protected virtual string ExpectedJsInstance => string.Empty;
        protected abstract string ExpectedBackgroundWorkerJs { get; }

        static string ReadFromEmbeddedResource(string name)
        {
            var assembly = typeof(BaseBackgroundSourceGeneratorTest).Assembly;
            using var fileStream = assembly.GetManifestResourceStream($"Blazor.BrowserExtension.Analyzer.Test.TestFiles.{name}")
                ?? throw new InvalidOperationException("Embedded resource not found");
            using var streamReader = new StreamReader(fileStream);
            return streamReader.ReadToEnd();
        }

        static string IndentContent(string content, int indent)
        {
            var indentation = "".PadRight(indent);
            return string.Join($"{Environment.NewLine}{indentation}", content.Split(Environment.NewLine));
        }

        static ReferenceAssemblies GetReferenceAssemblies()
        {
            var propertyName = $"Net{CommonTestHelper.TargetFrameworkMajorVersion}0";
            var property = typeof(ReferenceAssemblies.Net).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            return property?.GetValue(null) as ReferenceAssemblies
                ?? throw new InvalidOperationException($"ReferenceAssemblies for {propertyName} not found");
        }
    }
}
