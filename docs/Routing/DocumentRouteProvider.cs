using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace Docs.Routing
{
    public static class DocumentRouteProvider
    {
        const string ResourceNamePrefix = "Docs.Pages.";
        const string ResourceNameSuffix = ".md";
        const string NotFound = nameof(NotFound);

        private static Assembly CurrentAssembly => typeof(DocumentRouteProvider).Assembly;
        private static IEnumerable<DocumentRouteMetadata>? documentRoutes;
        public static IEnumerable<DocumentRouteMetadata> DocumentRoutes => documentRoutes ??= GetAllDocumentRoutes();

        private static List<DocumentRouteMetadata> GetAllDocumentRoutes()
            => [.. GetResourceDocuments()
                .Concat(GetRazorPages())
                .OrderBy(metadata => metadata.FileName)];

        private static IEnumerable<DocumentRouteMetadata> GetResourceDocuments() => CurrentAssembly
                .GetManifestResourceNames()
                .Where(resourceName => resourceName.StartsWith(ResourceNamePrefix) && resourceName.EndsWith(ResourceNameSuffix) && resourceName != $"{ResourceNamePrefix}{NotFound}{ResourceNameSuffix}")
                .Select(resourceName =>
                {
                    var fileName = resourceName[ResourceNamePrefix.Length..];
                    var unformattedDocumentName = ExtractDocumentNameWithoutOrder(fileName[..^ResourceNameSuffix.Length]);
                    var formattedDocumentName = FormatName(unformattedDocumentName, ' ');
                    var webPath = FormatName(unformattedDocumentName, '-', char.ToLowerInvariant);
                    webPath = EnsureValidWebPath(webPath);

                    return new DocumentRouteMetadata()
                    {
                        FileName = fileName,
                        Name = formattedDocumentName,
                        WebPath = webPath,
                        Depth = fileName.Count(c => c == '_'),
                        ResourceName = resourceName
                    };
                });

        private static IEnumerable<DocumentRouteMetadata> GetRazorPages() => CurrentAssembly.GetExportedTypes()
                .Where(type => type.FullName?.StartsWith(ResourceNamePrefix) == true && Attribute.IsDefined(type, typeof(RouteAttribute), false))
                .Select(type =>
                {
                    var fileName = type.FullName![ResourceNamePrefix.Length..].TrimStart('_');
                    var unformattedDocumentName = ExtractDocumentNameWithoutOrder(fileName);
                    var formattedDocumentName = FormatName(unformattedDocumentName, ' ');
                    var routePattern = type.GetCustomAttribute<RouteAttribute>(false)!.Template;
                    var webPath = routePattern.Trim('/');

                    return new DocumentRouteMetadata()
                    {
                        FileName = fileName + ".razor",
                        Name = formattedDocumentName,
                        WebPath = webPath,
                        Depth = fileName.Count(c => c == '_'),
                        ResourceName = type.FullName
                    };
                });

        private static string ExtractDocumentNameWithoutOrder(string documentName)
        {
            var index = documentName.LastIndexOf('_');
            return index == -1 ? documentName : documentName[(index + 1)..];
        }

        private static string FormatName(string documentName, char separator, Func<char, char>? transform = null)
        {
            if (documentName.Length < 2)
            {
                return documentName;
            }
            var sb = new StringBuilder();
            sb.Append(transform?.Invoke(documentName[0]) ?? documentName[0]);
            for (var i = 1; i < documentName.Length; ++i)
            {
                var c = documentName[i];
                if (char.IsUpper(c) && !char.IsUpper(documentName[i - 1]))
                {
                    sb.Append(separator);
                }
                sb.Append(transform?.Invoke(c) ?? c);
            }
            return sb.ToString();
        }

        private static string EnsureValidWebPath(string webPath) => webPath.Replace('.', '-');

        public static DocumentRouteMetadata? GetDocumentRouteMetadataFromResourceName(string? resourceName) => DocumentRoutes.FirstOrDefault(route => route.ResourceName == resourceName);

        public static DocumentRouteMetadata? GetDocumentRouteMetadataFromFileName(string? fileName) => DocumentRoutes.FirstOrDefault(route => route.FileName == fileName);

        public static DocumentRouteMetadata? GetDocumentRouteMetadataFromPath(string? webPath)
        {
            var pathToMatch = (webPath ?? string.Empty).Trim('/');
            return DocumentRoutes.FirstOrDefault(route => route.WebPath == pathToMatch);
        }

        public static Stream GetDocumentRouteResourceStream(DocumentRouteMetadata? documentRouteMetadata)
        {
            var resourceName = documentRouteMetadata?.ResourceName ?? $"{ResourceNamePrefix}NotFound{ResourceNameSuffix}";
            return CurrentAssembly.GetManifestResourceStream(resourceName)!;
        }
    }
}
