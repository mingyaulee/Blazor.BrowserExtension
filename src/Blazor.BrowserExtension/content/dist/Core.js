(async () => {
  const options = {
    ProjectName: "__ProjectName__",
    EnvironmentName: "__EnvironmentName__",
    CompressionEnabled: "__CompressionEnabled__"
  };

  let debugMode = false;
  if (typeof globalThis.chrome != "object" || !globalThis.chrome || !globalThis.chrome.runtime || !globalThis.chrome.runtime.id) {
    debugMode = true;
  }

  if (globalThis.ImportBrowserPolyfill !== false && !debugMode) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import('./lib/browser-polyfill.min.js');
  }

  const initializeInternal = (await import('./CoreInternal-5d5b8261.js')).initializeInternal;
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
  const browserExtension = initializeInternal(options, url, browserExtensionMode);

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(options.EnvironmentName);
  }
})();
