(async () => {
  console.warn("Core.js is deprecated. Use '_framework/blazor.webassembly.js' in the script tag.");

  let debugMode = false;
  const hasExtensionsApi = namespace => typeof globalThis[namespace] == "object" && globalThis[namespace]?.runtime?.id;
  if (!hasExtensionsApi("browser") && !hasExtensionsApi("chrome")) {
    debugMode = true;
  }

  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  let url;
  /** @type {import("./Modules/BrowserExtensionModes.js").BrowserExtensionMode} */
  let browserExtensionMode;
  if (!debugMode) {
    url = (globalThis.browser || globalThis.chrome).runtime.getURL("");
    browserExtensionMode = "Standard";
  } else {
    url = globalThis.location.origin + "/";
    browserExtensionMode = "Debug";
  }

  const configUrl = `${url}content/browserextension.config.json`;

  const configRequest = await fetch(configUrl);
  /** @type {import("./Modules/BrowserExtensionConfig.js").default} */
  const config = await configRequest.json();

  const blazorBrowserExtension = initializeInternal(config, url, browserExtensionMode);
  // Clear the BrowserExtension property for the module initializer to initialize it
  const browserExtension = blazorBrowserExtension.BrowserExtension;
  blazorBrowserExtension.BrowserExtension = null;

  if (debugMode) {
    blazorBrowserExtension.ImportBrowserPolyfill = false;
  }

  if (config.HasAppJs) {
    await import(`${url}app.js`);
  }

  if (blazorBrowserExtension.ImportBrowserPolyfill) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import("./lib/browser-polyfill.min.js");
  }

  if (blazorBrowserExtension.StartBlazorBrowserExtension) {
    await browserExtension.InitializeAsync();
  }
})();
