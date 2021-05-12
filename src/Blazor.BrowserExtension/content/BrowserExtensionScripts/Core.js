(async () => {
  const projectName = "__ProjectName__";

  if (globalThis.ImportBrowserPolyfill !== false) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import("./lib/browser-polyfill.min.js");
  }

  const initializeAsync = (await import("./Modules/CoreInternal.js")).initializeAsync;

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await initializeAsync(projectName);
  }
})();