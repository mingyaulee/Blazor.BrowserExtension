(async () => {
  const options = {
    ProjectName: "__ProjectName__",
    EnvironmentName: "__EnvironmentName__",
    CompressionEnabled: "__CompressionEnabled__"
  };

  const appDiv = document.createElement("div");
  appDiv.id = `${options.ProjectName}_app`;
  document.body.appendChild(appDiv);

  if (globalThis.ImportBrowserPolyfill !== false) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import('./lib/browser-polyfill.min.js');
  }

  const initializeInternal = (await import('./CoreInternal-5d5b8261.js')).initializeInternal;
  const url = globalThis.browser.runtime.getURL("");
  const browserExtension = initializeInternal(options, url, "ContentScript");

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(options.EnvironmentName);
  }
})();
