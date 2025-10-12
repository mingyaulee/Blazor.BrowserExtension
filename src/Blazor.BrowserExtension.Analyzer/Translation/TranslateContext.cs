using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazor.BrowserExtension.Analyzer.Translation
{
    internal sealed class TranslateContext(SemanticModel semanticModel)
    {
        private HashSet<SyntaxNode> JsAccessPathNodes { get; } = [];
        public Dictionary<string, string> ReferenceDictionary { get; } = [];
        public ParentExpressionType ParentExpressionType { get; set; }

        public ISymbol GetSymbol(SyntaxNode node)
            => semanticModel.GetSymbolInfo(node).Symbol;

        public TypeInfo GetType(SyntaxNode node)
            => semanticModel.GetTypeInfo(node);

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
            => JsAccessPathNodes.Contains(node);

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

        private static bool TryGetJsAccessPath(IEnumerable<AttributeData> attributes, out string jsAccessPath)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass.ToString() == "JsBind.Net.JsAccessPathAttribute")
                {
                    jsAccessPath = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    return !string.IsNullOrEmpty(jsAccessPath);
                }
                else if (attribute.AttributeClass.ToString() == "WebExtensions.Net.EnumValueAttribute")
                {
                    jsAccessPath = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (!string.IsNullOrEmpty(jsAccessPath))
                    {
                        jsAccessPath = $"\"{jsAccessPath}\"";
                        return true;
                    }
                }
            }

            jsAccessPath = null;
            return false;
        }
    }
}
