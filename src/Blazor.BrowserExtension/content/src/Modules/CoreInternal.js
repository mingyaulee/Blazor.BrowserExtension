import BrowserExtension from "./BrowserExtension.js";
import { initializeGlobalVariable } from "./GlobalVariableInitializer.js";

/**
 * @typedef {import("./BlazorBrowserExtension.js").default} BlazorBrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionMode} BrowserExtensionMode
 * @typedef {import("./BrowserExtensionConfig.js").default} BrowserExtensionConfig
 */

/**
 * Initializes the Blazor Browser Extension internally
 * @param {BrowserExtensionConfig} config The initialization options.
 * @param {string} browserExtensionUrl The browser extension url.
 * @param {BrowserExtensionMode} browserExtensionMode The browser extension mode.
 * @returns {BlazorBrowserExtension}
 */
export function initializeInternal(config, browserExtensionUrl, browserExtensionMode) {
  const browserExtension = new BrowserExtension(browserExtensionUrl, browserExtensionMode, config);
  return initializeGlobalVariable(browserExtension);
}