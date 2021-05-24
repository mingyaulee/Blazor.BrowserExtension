(async () => {
  const projectName = "__ProjectName__";
  const environmentName = "__EnvironmentName__";

  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  const url = globalThis.location.origin + "/";
  const browserExtension = initializeInternal(projectName, url, "Debug");

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(environmentName);
  }
})();