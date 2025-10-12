namespace Blazor.BrowserExtension.IntegrationTest
{
    public partial class BackgroundWorker : BackgroundWorkerBase
    {
        [BackgroundWorkerMain]
        public override void Main()
            => WebExtensions.Runtime.OnInstalled.AddListener(OnInstalled);

        async Task OnInstalled()
        {
            var indexPageUrl = WebExtensions.Runtime.GetURL("index.html");
            await WebExtensions.Tabs.Create(new()
            {
                Url = indexPageUrl
            });
        }
    }
}
