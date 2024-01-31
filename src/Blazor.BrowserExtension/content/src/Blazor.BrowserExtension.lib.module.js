import { initializeInternal } from "./Modules/CoreInternal.js";

let debugMode = false;
const hasExtensionsApi = (namespace) => typeof globalThis[namespace] == "object" && globalThis[namespace]?.runtime?.id;
if (!hasExtensionsApi("browser") && !hasExtensionsApi("chrome")) {
  debugMode = true;
}

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

/** @type {import("./Modules/BrowserExtensionConfig.js").default} */
let config;
let appJs;

/**
 * Called before Blazor starts.
 * @param {object} options Blazor WebAssembly start options. Refer to https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Platform/WebAssemblyStartOptions.ts
 * @param {object} extensions Extensions added during publishing
 */
export async function beforeStart(options, extensions) {
  const configUrl = `${url}content/browserextension.config.json`;
  const configRequest = await fetch(configUrl);

  /** @type {import("./Modules/BrowserExtensionConfig.js").default} */
  config = await configRequest.json();

  /** @type {import("./Modules/BlazorBrowserExtension.js").default} */
  const blazorBrowserExtension = options.IsContentScript ? options.BlazorBrowserExtension : initializeInternal(config, url, browserExtensionMode);

  if (debugMode) {
    blazorBrowserExtension.ImportBrowserPolyfill = false;
  }

  if (config.HasAppJs) {
    appJs = await import(`${url}app.js`);
  }

  if (blazorBrowserExtension.ImportBrowserPolyfill) {
    // import browser extension API polyfill
    // @ts-ignore JS is not a module
    await import("./lib/browser-polyfill.min.js");
  }

  await blazorBrowserExtension.BrowserExtension.InitializeCoreAsync(options);

  if (appJs?.beforeStart) {
    const beforeStartReturn = appJs.beforeStart(options, extensions, blazorBrowserExtension);
    if (beforeStartReturn instanceof Promise) {
      await beforeStartReturn;
    }
  }
}

/**
 * Called after Blazor is ready to receive calls from JS.
 * @param {any} blazor The Blazor instance
 */
export function afterStarted(blazor) {
  if (appJs?.afterStarted) {
    return appJs.afterStarted(blazor);
  }
}
