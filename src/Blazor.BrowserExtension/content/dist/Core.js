(async () => {
  let debugMode = false;
  if (typeof globalThis.chrome != "object" || !globalThis.chrome || !globalThis.chrome.runtime || !globalThis.chrome.runtime.id) {
    debugMode = true;
  }

  if (globalThis.ImportBrowserPolyfill !== false && !debugMode) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import('./lib/browser-polyfill.min.js');
  }

  const initializeInternal = (await import('./CoreInternal.js')).initializeInternal;
  let url;
  /** @type {import("./Modules/BrowserExtensionModes").BrowserExtensionMode} */
  let browserExtensionMode;
  if (!debugMode) {
    url = globalThis.browser.runtime.getURL("");
    browserExtensionMode = "Standard";
  } else {
    url = globalThis.location.origin + "/";
    browserExtensionMode = "Debug";
  }

  let configUrl;
  if (debugMode) {
    configUrl = `${url}_content/browserextension.config.json`;
  } else {
    configUrl = `${url}content/browserextension.config.json`;
  }

  const configRequest = await fetch(configUrl);
  const config = await configRequest.json();

  const browserExtension = initializeInternal(config, url, browserExtensionMode);

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(config.EnvironmentName);
  }
})();
