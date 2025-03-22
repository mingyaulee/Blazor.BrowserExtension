using Microsoft.CodeAnalysis;

namespace Blazor.BrowserExtension.Analyzer
{
    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateTranslationError(Location location, string message)
        {
            return Diagnostic.Create(TranslationDiagnostic, location, message);
        }

        static DiagnosticDescriptor TranslationDiagnostic = new(
            id: "BROWSEREXT001",
            title: "Background Worker Translation Error",
            messageFormat: "Failed to translate expression: {0}",
            category: "CodeGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }
}
