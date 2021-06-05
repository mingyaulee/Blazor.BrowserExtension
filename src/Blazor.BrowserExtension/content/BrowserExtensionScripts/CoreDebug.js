(async () => {
  const options = {
    ProjectName: "__ProjectName__",
    EnvironmentName: "__EnvironmentName__",
    CompressionEnabled: "__CompressionEnabled__"
  };

  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  const url = globalThis.location.origin + "/";
  const browserExtension = initializeInternal(options, url, "Debug");

  if (globalThis.StartBlazorBrowserExtension !== false) {
    await browserExtension.InitializeAsync(options.EnvironmentName);
  }
})();