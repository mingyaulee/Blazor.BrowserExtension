import BlazorBrowserExtension from "./BlazorBrowserExtension.js";
import BrowserExtension from "./BrowserExtension.js";
import BrowserExtensionModes from "./BrowserExtensionModes.js";

/**
 * Gets the browser extension mode.
 * @param {string} extensionUrl The extension URL.
 * @param {string} currentUrl The current URL.
 * @returns {string}
 */
function getBrowserExtensionMode(extensionUrl, currentUrl) {
  // Set browser extension mode to ContentScript for URL that does not match the browser extension URL
  if (extensionUrl.startsWith(currentUrl)) {
    return BrowserExtensionModes.Standard;
  } else {
    return BrowserExtensionModes.ContentScript;
  }
}

/**
 * Initializes the Blazor Browser Extension application
 * @param {string} projectName The project name.
 * @returns {Promise<BrowserExtension>}
 */
async function initializeAsync(projectName) {
  // each extension's definition is stored in BlazorBrowserExtension[name]
  const url = globalThis.browser.runtime.getURL("");
  const currentUrl = globalThis.location.origin;
  const browserExtension = new BrowserExtension(url, getBrowserExtensionMode(url, currentUrl));
  globalThis.BlazorBrowserExtension[projectName] = browserExtension;

  // import WebExtension.Net JS
  await import(`${url}WebExtensionScripts/WebExtensionNet.js`);

  // import blazor.webassembly.js
  const blazorScript = document.createElement("script");
  blazorScript.src = `${url}framework/blazor.webassembly.js`;
  blazorScript.defer = true;
  // Blazor is set to not auto start, so that we can start it with different environment name
  blazorScript.setAttribute("autostart", "false");
  await browserExtension.AppendElementToDocumentAsync(blazorScript);

  // Start Blazor
  if (browserExtension.Mode === BrowserExtensionModes.Standard) {
    // Blazor script tag is injected, run start after the script is loaded
    blazorScript.onload = () => {
      globalThis.Blazor.start({
        environment: browserExtension.Mode
      });
    }
  } else {
    // Blazor is imported, we can start it right away
    globalThis.Blazor.start({
      environment: browserExtension.Mode,
      loadBootResource: function (resourceType, resourceName, defaultUri, integrity) {
        if (resourceType === "dotnetjs") {
          return `${url}framework/${resourceName}`;
        }
        return defaultUri;
      }
    });
  }

  return browserExtension;
}

/** @type {BlazorBrowserExtension} */
let blazorBrowserExtension;

// initialize global property BlazorBrowserExtension
if (!globalThis.hasOwnProperty("BlazorBrowserExtension")) {
  blazorBrowserExtension = new BlazorBrowserExtension();
  blazorBrowserExtension.Modes = BrowserExtensionModes;
  globalThis.BlazorBrowserExtension = blazorBrowserExtension;
} else {
  blazorBrowserExtension = /** @type {BlazorBrowserExtension} */ (globalThis.BlazorBrowserExtension);
}

blazorBrowserExtension.InitializeAsync = initializeAsync;

export default initializeAsync;