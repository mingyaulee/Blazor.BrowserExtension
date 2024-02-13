using System.IO;
using Docs.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Docs.Routing
{
    [Route("/{*pageRoute}")]
    public class RoutingPage : ComponentBase
    {
        [Parameter]
        public string? PageRoute { get; set; }

        [Inject]
        public IJSInProcessRuntime JsRuntime { get; set; } = default!;

        [Inject]
        public DocumentRouteEvent DocumentRouteEvent { get; set; } = default!;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var documentRouteMetadata = DocumentRouteProvider.GetDocumentRouteMetadataFromPath(PageRoute);
            using var stream = DocumentRouteProvider.GetDocumentRouteResourceStream(documentRouteMetadata);
            using var streamReader = new StreamReader(stream);
            var markdown = MarkdownRenderer.Render(streamReader.ReadToEnd());
            builder.AddMarkupContent(0, markdown);
            DocumentRouteEvent.TriggerChange(documentRouteMetadata);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            JsRuntime.InvokeVoid("globalThis.appUtils.onPageRender");
        }
    }
}
