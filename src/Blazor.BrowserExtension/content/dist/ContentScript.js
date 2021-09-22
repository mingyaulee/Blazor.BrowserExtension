(async () => {
  if (globalThis.ImportBrowserPolyfill !== false) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import('./lib/browser-polyfill.min.js');
  }

  const initializeInternal = (await import('./CoreInternal.js')).initializeInternal;
  const url = globalThis.browser.runtime.getURL("");

  const configRequest = await fetch(`${url}content/browserextension.config.json`);
  const config = await configRequest.json();

  const browserExtension = initializeInternal(config, url, "ContentScript");

  const appDiv = document.createElement("div");
  appDiv.id = `${config.ProjectName}_app`;
  document.body.appendChild(appDiv);

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(config.EnvironmentName);
  }
})();
