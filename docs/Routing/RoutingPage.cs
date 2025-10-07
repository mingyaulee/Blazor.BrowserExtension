using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
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
            var targetFramework = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkDisplayName!.ToLower().Replace(" ", string.Empty).TrimStart('.');
            var targetFrameworkMajorVersion = targetFramework.Split('.')[0][3..];
            var documentRouteMetadata = DocumentRouteProvider.GetDocumentRouteMetadataFromPath(PageRoute);
            using var stream = DocumentRouteProvider.GetDocumentRouteResourceStream(documentRouteMetadata);
            using var streamReader = new StreamReader(stream);
            var markdown = streamReader.ReadToEnd()
                .Replace("[NetVersion]", targetFrameworkMajorVersion);
            var markup = MarkdownRenderer.Render(markdown);
            builder.AddMarkupContent(0, markup);
            DocumentRouteEvent.TriggerChange(documentRouteMetadata);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            JsRuntime.InvokeVoid("globalThis.appUtils.onPageRender");
        }
    }
}
