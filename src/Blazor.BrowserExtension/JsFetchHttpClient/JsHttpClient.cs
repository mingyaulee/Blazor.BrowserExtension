using Blazor.BrowserExtension.JsFetchHttpClient;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace System.Net.Http
{
    public class JsHttpClient : HttpClient
    {
        private static JsHttpMessageHandler jsHttpMessageHandler;
        public JsHttpClient(IServiceProvider serviceProvider) : base(CreateMessageHandler(serviceProvider), false)
        {
        }

        private static JsHttpMessageHandler CreateMessageHandler(IServiceProvider serviceProvider)
        {
            if (jsHttpMessageHandler is null)
            {
                jsHttpMessageHandler = new JsHttpMessageHandler(serviceProvider.GetRequiredService<IJSRuntime>());
            }
            return jsHttpMessageHandler;
        }

        public new Uri BaseAddress
        {
            get
            {
                return jsHttpMessageHandler.BaseAddress;
            }
            set
            {
                base.BaseAddress = jsHttpMessageHandler.ValidBaseAddress;
                jsHttpMessageHandler.BaseAddress = value;
            }
        }
    }
}
