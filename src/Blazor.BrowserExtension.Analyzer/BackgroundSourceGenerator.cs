using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Blazor.BrowserExtension.Analyzer.Translation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazor.BrowserExtension.Analyzer
{
    [Generator(LanguageNames.CSharp)]
    internal class BackgroundSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var backgroundWorkerSyntaxSource = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "Blazor.BrowserExtension.BackgroundWorkerMainAttribute",
                predicate: static (syntaxNode, _) => syntaxNode is MethodDeclarationSyntax,
                transform: Transform);

            context.RegisterSourceOutput(backgroundWorkerSyntaxSource, Generate);
        }

        private static SourceModel Transform(GeneratorAttributeSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            var mainMethod = (MethodDeclarationSyntax)syntaxContext.TargetNode;
            var expressions = mainMethod?.ExpressionBody is ArrowExpressionClauseSyntax arrowExpression ?
                [arrowExpression.Expression] :
                mainMethod?.Body?.ChildNodes() ?? [];

            var context = new TranslateContext(syntaxContext.SemanticModel);
            var translatedExpressions = TranslateExpressions(expressions, context, out var errors);
            var declaringClass = syntaxContext.TargetSymbol.ContainingType;

            return new SourceModel()
            {
                Namespace = declaringClass.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                ClassName = declaringClass.Name,
                TranslatedMain = translatedExpressions,
                ReferenceDictionary = context.ReferenceDictionary,
                FilePath = syntaxContext.SemanticModel.SyntaxTree.FilePath,
                TranslationErrors = errors
            };
        }

        private static string TranslateExpressions(IEnumerable<SyntaxNode> expressions, TranslateContext context, out IEnumerable<(Location, string)> errors)
        {
            var errorList = new List<(Location, string)>();
            var translatedExpressions = new StringBuilder();
            foreach (var expression in expressions)
            {
                try
                {
                    translatedExpressions.AppendLine(ExpressionTranslator.Translate(expression, context));
                }
                catch (Exception exception)
                {
                    errorList.Add((expression.GetLocation(), exception.Message));
                }
            }
            errors = errorList;
            return translatedExpressions.ToString();
        }

        private static void Generate(SourceProductionContext generateContext, SourceModel source)
        {
            if (source.TranslationErrors.Any())
            {
                foreach (var (location, message) in source.TranslationErrors)
                {
                    generateContext.ReportDiagnostic(DiagnosticFactory.CreateTranslationError(location, message));
                }
                return;
            }

#pragma warning disable RS1035 // Do not use APIs banned for analyzers
            // Ideally we would want to output this directly from the source generator, this is not supported currently
            // https://github.com/dotnet/roslyn/issues/57608
            var rootProjectDir = Path.GetDirectoryName(source.FilePath);
            while (Directory.GetFiles(rootProjectDir, "*.csproj").Length == 0)
            {
                rootProjectDir = Path.GetDirectoryName(rootProjectDir);
                if (string.IsNullOrEmpty(rootProjectDir))
                {
                    throw new InvalidOperationException("Unable to locate root project directory.");
                }
            }
            var jsFilePath = Path.Combine(rootProjectDir, "obj/BackgroundWorkerMain.generated.js");
            File.WriteAllText(jsFilePath, source.TranslatedMain);
#pragma warning restore RS1035 // Do not use APIs banned for analyzers

            const string newLine = @"
";
            var isGlobalNamespace = string.IsNullOrEmpty(source.Namespace);
            var indentation = "".PadRight(isGlobalNamespace ? 8 : 12);
            var generatedDictionary = string.Join($"{newLine}{indentation}", source.ReferenceDictionary.Select(keyValuePair => $$"""{ "{{keyValuePair.Key}}", {{keyValuePair.Value}} },"""));

            if (isGlobalNamespace)
            {
                generateContext.AddSource($"{source.ClassName}.generated.cs", $$"""
                    using System.Collections.Generic;

                    public partial class {{source.ClassName}}
                    {
                        protected override Dictionary<string, object> JsInstance => new()
                        {
                            {{generatedDictionary}}
                        };
                    }
                    """);
            }
            else
            {
                generateContext.AddSource($"{source.ClassName}.generated.cs", $$"""
                    using System.Collections.Generic;

                    namespace {{source.Namespace}}
                    {
                        public partial class {{source.ClassName}}
                        {
                            protected override Dictionary<string, object> JsInstance => new()
                            {
                                {{generatedDictionary}}
                            };
                        }
                    }
                    """);
            }
        }

        private sealed class SourceModel
        {
            public string Namespace { get; set; }
            public string ClassName { get; set; }
            public string TranslatedMain { get; set; }
            public Dictionary<string, string> ReferenceDictionary { get; set; }
            public string FilePath { get; set; }
            public IEnumerable<(Location, string)> TranslationErrors { get; set; }
        }
    }
}
