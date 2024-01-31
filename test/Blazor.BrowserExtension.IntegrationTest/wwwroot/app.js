/**
 * Called before Blazor starts.
 * @param {object} options Blazor WebAssembly start options. Refer to https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Platform/WebAssemblyStartOptions.ts
 * @param {object} extensions Extensions added during publishing
 * @param {object} blazorBrowserExtension Blazor browser extension
*/
export function beforeStart(options, extensions, blazorBrowserExtension) {
  if (blazorBrowserExtension.BrowserExtension.Mode === blazorBrowserExtension.Modes.ContentScript) {
    const appDiv = document.createElement("div");
    appDiv.id = "Blazor_BrowserExtension_IntegrationTest_app";
    document.body.appendChild(appDiv);
  }
}