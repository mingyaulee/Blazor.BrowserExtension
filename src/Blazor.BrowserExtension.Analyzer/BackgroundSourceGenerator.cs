using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
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
                transform: static (syntaxContext, _) =>
                {
                    var mainMethod = (MethodDeclarationSyntax)syntaxContext.TargetNode;
                    var expressions = GetBodyExpressions(mainMethod);
                    var context = new TranslateContext(syntaxContext.SemanticModel);
                    var translatedExpressions = TranslateExpressions(expressions, context);
                    var declaringClass = syntaxContext.TargetSymbol.ContainingType;

                    return new SourceModel()
                    {
                        Namespace = declaringClass.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                        ClassName = declaringClass.Name,
                        TranslatedMain = translatedExpressions,
                        ReferenceDictionary = context.ReferenceDictionary,
                        FilePath = syntaxContext.SemanticModel.SyntaxTree.FilePath
                    };
                });

            context.RegisterSourceOutput(backgroundWorkerSyntaxSource, (generateContext, source) =>
            {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
                var generatedDictionary = string.Join(Environment.NewLine, source.ReferenceDictionary.Select(keyValuePair => $$"""{ "{{keyValuePair.Key}}", {{keyValuePair.Value}} },"""));

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
            });
        }

        private static string TranslateExpressions(IEnumerable<SyntaxNode> expressions, TranslateContext context)
        {
            var translatedExpressions = new StringBuilder();
            foreach (var expression in expressions)
            {
                translatedExpressions.AppendLine(ExpressionTranslator.TranslateExpressions(expression, context));
            }
            return translatedExpressions.ToString();
        }

        private static IEnumerable<SyntaxNode> GetBodyExpressions(MethodDeclarationSyntax mainMethod)
        {
            if (mainMethod?.ExpressionBody is ArrowExpressionClauseSyntax arrowExpression)
            {
                return [arrowExpression.Expression];
            }

            return mainMethod?.Body?.ChildNodes() ?? Enumerable.Empty<SyntaxNode>();
        }

        private sealed class SourceModel
        {
            public string Namespace { get; set; }
            public string ClassName { get; set; }
            public string TranslatedMain { get; set; }
            public Dictionary<string, string> ReferenceDictionary { get; set; }
            public string FilePath { get; set; }
        }
    }
}
