using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                    var translatedExpressions = new StringBuilder();
                    var context = new TranslateContext(syntaxContext.SemanticModel);
                    foreach (var translated in TranslateExpressions(expressions, context))
                    {
                        translatedExpressions.AppendLine(translated);
                    }
                    var declaringClass = syntaxContext.TargetSymbol.ContainingType;

                    return new SourceModel()
                    {
                        Namespace = declaringClass.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                        ClassName = declaringClass.Name,
                        TranslatedMain = translatedExpressions.ToString(),
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

        private static IEnumerable<string> TranslateExpressions(IEnumerable<SyntaxNode> expressions, TranslateContext context)
        {
            foreach (var expression in expressions)
            {
                yield return TranslateExpressions(expression, context);
            }
        }

        [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "Examples of code syntax being translated")]
        private static string TranslateExpressions(SyntaxNode expression, TranslateContext context)
        {
            // In general, 'Value' in the examples can be a Constant value or a value returned from a method invocation
            if (expression is ExpressionStatementSyntax expressionStatement)
            {
                // Can be any statement
                // SomeMethod(args);
                return $"{TranslateExpressions(expressionStatement.Expression, context)};";
            }
            else if (expression is LocalDeclarationStatementSyntax localDeclarationStatement)
            {
                // var variable1 = Value1, variable2 = Value2;
                var declarations = localDeclarationStatement.Declaration.Variables.Select(variable => TranslateExpressions(variable, context));
                return $"let {string.Join(", ", declarations)};";
            }
            else if (expression is VariableDeclaratorSyntax variableDeclarator && variableDeclarator.Initializer is EqualsValueClauseSyntax initializeEquals)
            {
                // variable1 = Value1
                var equalsValue = TranslateExpressions(initializeEquals.Value, context);
                return $"{variableDeclarator.Identifier.Text} = {equalsValue}";
            }
            else if (expression is AssignmentExpressionSyntax assignmentExpression)
            {
                // obj.Property1 = Value1
                // or
                // obj[index1] = Value1
                var assignValue = TranslateExpressions(assignmentExpression.Right, context);
                var assignTo = TranslateExpressions(assignmentExpression.Left, context);

                if (assignTo == "_")
                {
                    // Ignore discard assignments
                    return assignValue;
                }

                return $"{assignTo} = {assignValue}";
            }
            else if (expression is InvocationExpressionSyntax invocationExpression)
            {
                // obj.Method1(arg1, arg2)
                var args = invocationExpression.ArgumentList.Arguments.Select(arg => TranslateExpressions(arg, context));
                var invoked = TranslateExpressions(invocationExpression.Expression, context);
                return $"{invoked}({string.Join(", ", args)})";
            }
            else if (expression is MemberAccessExpressionSyntax memberAccessExpression)
            {
                // obj.Property1
                // or
                // obj.Method1
                var target = TranslateExpressions(memberAccessExpression.Expression, context);
                var jsAccessPath = context.GetJsAccessPath(expression, context.GetSymbol(memberAccessExpression));
                if (jsAccessPath is not null && !context.IsJsAccessPath(memberAccessExpression.Expression))
                {
                    return jsAccessPath;
                }

                var member = jsAccessPath ?? memberAccessExpression.Name.ToString();
                return $"{target}.{member}";
            }
            else if (expression is ArgumentSyntax argument)
            {
                // argument of a method invocation
                var type = context.GetType(argument.Expression);
                var translatedArgument = TranslateExpressions(argument.Expression, context);
                if (!context.IsJsAccessPath(argument.Expression) && argument.Expression is not LiteralExpressionSyntax)
                {
                    // Pass to JS during initialization
                    var referenceKey = context.AddReference(translatedArgument, $"({type.ConvertedType}){translatedArgument}");
                    return $"fromReference('{referenceKey}')";
                }

                return translatedArgument;
            }
            else if (expression is IdentifierNameSyntax identifierName)
            {
                var symbol = context.GetSymbol(identifierName);
                var jsAccessPath = context.GetJsAccessPath(expression, symbol);
                if (!string.IsNullOrEmpty(jsAccessPath))
                {
                    return jsAccessPath;
                }

                return identifierName.Identifier.Text;
            }
            else if (expression is LiteralExpressionSyntax literalExpressionSyntax)
            {
                return literalExpressionSyntax.Token.Text;
            }

            throw new InvalidOperationException($"Unmapped expression type {expression.GetType().Name}. Please raise an issue in GitHub with your code in Main.");
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
            public Dictionary<string, string> ReferenceDictionary { get;set; }
            public string FilePath { get; set; }
        }

        private sealed class TranslateContext(SemanticModel semanticModel)
        {
            private HashSet<SyntaxNode> JsAccessPathNodes { get; } = [];
            public Dictionary<string, string> ReferenceDictionary { get; } = [];

            public ISymbol GetSymbol(SyntaxNode node)
            {
                return semanticModel.GetSymbolInfo(node).Symbol;
            }

            public TypeInfo GetType(SyntaxNode node)
            {
                return semanticModel.GetTypeInfo(node);
            }

            public string AddReference(string key, string value)
            {
                if (ReferenceDictionary.TryGetValue(key, out var existingValue))
                {
                    if (existingValue == value)
                    {
                        return key;
                    }

                    return AddReference(key + "0", value);
                }

                ReferenceDictionary.Add(key, value);
                return key;
            }

            public bool IsJsAccessPath(ExpressionSyntax node)
            {
                return JsAccessPathNodes.Contains(node);
            }

            public string GetJsAccessPath(SyntaxNode node, ISymbol symbol)
            {
                if (symbol is not null && TryGetJsAccessPath(symbol.GetAttributes(), out var jsAccessPath))
                {
                    JsAccessPathNodes.Add(node);
                    return jsAccessPath;
                }

                if (symbol is IPropertySymbol propertySymbol && TryGetJsAccessPath(propertySymbol.Type.GetAttributes(), out jsAccessPath))
                {
                    JsAccessPathNodes.Add(node);
                    return jsAccessPath;
                }

                var symbolName = GetSymbolFullName(symbol);
                if (symbolName is not null && SystemTranslations.TryGetValue(symbolName, out var translation))
                {
                    JsAccessPathNodes.Add(node);
                    return translation;
                }

                return null;
            }

            private static string GetSymbolFullName(ISymbol symbol)
            {
                if (symbol is INamedTypeSymbol)
                {
                    return $"{symbol.ContainingNamespace}.{symbol.Name}";
                }
                else if (symbol is IMethodSymbol methodSymbol)
                {
                    return $"{GetSymbolFullName(methodSymbol.ContainingType)}.{methodSymbol.Name}";
                }
                else if (symbol is IPropertySymbol propertySymbol)
                {
                    return GetSymbolFullName(propertySymbol.Type);
                }

                return null;
            }

            private static Dictionary<string, string> SystemTranslations = new()
            {
                { "System.Console", "console" },
                { "System.Console.WriteLine", "log" },
                { "System.Console.Write", "log" },
                { "Microsoft.Extensions.Logging.ILogger", "console" },
                { "Microsoft.Extensions.Logging.LoggerExtensions.LogInformation", "log" },
                { "Microsoft.Extensions.Logging.LoggerExtensions.LogWarning", "warn" },
                { "Microsoft.Extensions.Logging.LoggerExtensions.LogError", "error" },
                { "Microsoft.Extensions.Logging.LoggerExtensions.LogCritical", "error" },
            };

            private static bool TryGetJsAccessPath(IEnumerable<AttributeData> attributes, out string jsAccessPath)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.ToString() == "JsBind.Net.JsAccessPathAttribute")
                    {
                        jsAccessPath = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                        return !string.IsNullOrEmpty(jsAccessPath);
                    }
                }

                jsAccessPath = null;
                return false;
            }
        }
    }
}
