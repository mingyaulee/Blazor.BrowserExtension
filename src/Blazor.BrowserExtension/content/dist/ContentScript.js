(async () => {
  const initializeInternal = (await import('./CoreInternal.js')).initializeInternal;
  const url = (globalThis.browser || globalThis.chrome).runtime.getURL("");

  const configRequest = await fetch(`${url}content/browserextension.config.json`);
  const config = await configRequest.json();

  const blazorBrowserExtension = initializeInternal(config, url, "ContentScript");

  if (config.HasAppJs) {
    await import(`${url}app.js`);
  }

  if (blazorBrowserExtension.ImportBrowserPolyfill) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import('./lib/browser-polyfill.min.js');
  }

  if (blazorBrowserExtension.StartBlazorBrowserExtension) {
    await blazorBrowserExtension.BrowserExtension.InitializeAsync(config.EnvironmentName);
  }
})();
