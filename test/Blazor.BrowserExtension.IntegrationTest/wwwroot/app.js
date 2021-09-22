if (globalThis.BlazorBrowserExtension.BrowserExtension.Mode === globalThis.BlazorBrowserExtension.Modes.ContentScript) {
  const appDiv = document.createElement("div");
  appDiv.id = "Blazor_BrowserExtension_IntegrationTest_app";
  document.body.appendChild(appDiv);
}
