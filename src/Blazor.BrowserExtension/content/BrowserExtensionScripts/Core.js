(async () => {
  const options = {
    ProjectName: "__ProjectName__",
    EnvironmentName: "__EnvironmentName__",
    CompressionEnabled: "__CompressionEnabled__"
  };

  if (globalThis.ImportBrowserPolyfill !== false) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import("./lib/browser-polyfill.min.js");
  }

  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  const url = globalThis.browser.runtime.getURL("");
  const browserExtension = initializeInternal(options, url, "Standard");

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(options.EnvironmentName);
  }
})();