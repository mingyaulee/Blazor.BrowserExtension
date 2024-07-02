import BlazorBrowserExtension from "./BlazorBrowserExtension.js";
import BrowserExtensionModes from "./BrowserExtensionModes.js";

/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 */

/**
 * Initializes the Blazor Browser Extension global variable
 * @param {BrowserExtension} browserExtension The browser extension.
 * @returns {BlazorBrowserExtension}
 */
export function initializeGlobalVariable(browserExtension) {
  /** @type {BlazorBrowserExtension} */
  let blazorBrowserExtension;

  // initialize global property BlazorBrowserExtension
  if (!globalThis.hasOwnProperty("BlazorBrowserExtension")) {
    blazorBrowserExtension = new BlazorBrowserExtension();
    blazorBrowserExtension.Modes = BrowserExtensionModes;
    globalThis.BlazorBrowserExtension = blazorBrowserExtension;
  } else {
    blazorBrowserExtension = /** @type {BlazorBrowserExtension} */(globalThis.BlazorBrowserExtension);
  }

  if (blazorBrowserExtension.BrowserExtension) {
    // Extensions execution should be isolated so this property should be null upon initialization.
    throw new Error("Browser extension cannot be loaded.");
  }

  blazorBrowserExtension.BrowserExtension = browserExtension;

  return blazorBrowserExtension;
}