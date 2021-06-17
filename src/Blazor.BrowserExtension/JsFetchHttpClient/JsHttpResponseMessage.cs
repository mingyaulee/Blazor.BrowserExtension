using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazor.BrowserExtension.JsFetchHttpClient
{
    internal class JsHttpResponseMessage : HttpResponseMessage
    {
        private readonly IJSObjectReference response;
        private readonly CancellationToken cancellationToken;

        public JsHttpResponseMessage(IJSObjectReference response, CancellationToken cancellationToken)
        {
            this.response = response;
            this.cancellationToken = cancellationToken;
        }

        public async Task InitializeAsync()
        {
            var statusCode = await response.InvokeAsync<int>("GetStatusCode", cancellationToken);
            StatusCode = (HttpStatusCode)statusCode;
            var responseText = await response.InvokeAsync<string>("GetResponseText", cancellationToken);
            Content = new StringContent(responseText);
            ReasonPhrase = await response.InvokeAsync<string>("GetStatusText", cancellationToken);
        }
    }
}
