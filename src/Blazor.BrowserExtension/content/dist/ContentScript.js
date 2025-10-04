(async () => {
  const initializeInternal = (await import('./CoreInternal.js')).initializeInternal;
  const url = (globalThis.browser || globalThis.chrome).runtime.getURL("");
  const documentBaseUri = new URL("/", document.baseURI).toString();

  const configRequest = await fetch(`${url}content/browserextension.config.json`);
  const config = await configRequest.json();

  const blazorBrowserExtension = initializeInternal(config, url, "ContentScript");

  globalThis.importProxy = (module) => {
    if (module.startsWith(documentBaseUri)) {
      module = new URL(module.substring(documentBaseUri.length), url);
    }

    return import(module);
  };

  await blazorBrowserExtension.BrowserExtension.InitializeContentScriptAsync({
    BlazorBrowserExtension: blazorBrowserExtension
  });
})();
