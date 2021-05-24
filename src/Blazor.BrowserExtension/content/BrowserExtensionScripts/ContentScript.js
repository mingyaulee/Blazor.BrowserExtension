(async () => {
  const projectName = "__ProjectName__";
  const environmentName = "__EnvironmentName__";

  const appDiv = document.createElement("div");
  appDiv.id = `${projectName}_app`;
  document.body.appendChild(appDiv);

  if (globalThis.ImportBrowserPolyfill !== false) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import("./lib/browser-polyfill.min.js");
  }

  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  const url = globalThis.browser.runtime.getURL("");
  const browserExtension = initializeInternal(projectName, url, "ContentScript");

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(environmentName);
  }
})();