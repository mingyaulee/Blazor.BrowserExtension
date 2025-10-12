using Markdig;

namespace Docs.Rendering
{
    public static class MarkdownRenderer
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .UsePipeTables()
            .UseBootstrap()
            .Use<LinkExtension>()
            .Build();

        public static string Render(string source)
            => Markdown.ToHtml(source, pipeline);
    }
}
