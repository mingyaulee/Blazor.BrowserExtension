using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazor.BrowserExtension.JsFetchHttpClient
{
    internal class JsHttpMessageHandler : HttpMessageHandler
    {
        private readonly Task<IJSObjectReference> jsModule;
        internal Uri ValidBaseAddress = new("https://non-existent-url");

        public JsHttpMessageHandler(IJSRuntime jsRuntime)
        {
            jsModule = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./BrowserExtensionScripts/JsHttpClient.js").AsTask();
        }

        public Uri BaseAddress { get; set; }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = GetRequestUri(request);
            var jsHttpClientModule = await jsModule;
            var fetchOptions = GetFetchOptions(request);
            var response = await jsHttpClientModule.InvokeAsync<IJSObjectReference>("Fetch", cancellationToken, requestUri, fetchOptions);
            var responseMessage = new JsHttpResponseMessage(response, cancellationToken)
            {
                RequestMessage = request
            };
            await responseMessage.InitializeAsync();
            return responseMessage;
        }

        private Uri GetRequestUri(HttpRequestMessage request)
        {
            if (BaseAddress is not null && request.RequestUri.Host == ValidBaseAddress.Host)
            {
                return new Uri(BaseAddress, ValidBaseAddress.MakeRelativeUri(request.RequestUri));
            }

            return request.RequestUri;
        }

        private static async Task<IDictionary<string, object>> GetFetchOptions(HttpRequestMessage request)
        {
            var fetchOptions = new Dictionary<string, object>(request.Options)
            {
                ["method"] = request.Method.Method,
                ["headers"] = request.Headers.ToDictionary(header => header.Key, header => string.Join(',', header.Value))
            };

            if (request.Content is not null)
            {
                fetchOptions["body"] = await request.Content.ReadAsStringAsync();
            }

            return fetchOptions;
        }
    }
}
