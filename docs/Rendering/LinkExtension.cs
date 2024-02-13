using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Docs.Routing;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Docs.Rendering
{
    public class LinkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed += OnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private void OnDocumentProcessed(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                if (node is LinkInline link)
                {
                    if (link.Url is not null && IsDocument(link.Url, out var documentRouteMetadata))
                    {
                        link.Url = link.Url.Replace(documentRouteMetadata.FileName, documentRouteMetadata.WebPath);
                    }
                    else
                    {
                        link.GetAttributes().AddProperty("target", "_blank");
                    }
                }
            }
        }

        static bool IsDocument(string url, [NotNullWhen(true)] out DocumentRouteMetadata? documentRouteMetadata)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                documentRouteMetadata = null;
                return false;
            }

            var hashIndex = url.IndexOf('#');
            url = hashIndex == -1 ? url : url[..hashIndex];
            documentRouteMetadata = DocumentRouteProvider.GetDocumentRouteMetadataFromFileName(url);
            return documentRouteMetadata is not null;
        }
    }
}
