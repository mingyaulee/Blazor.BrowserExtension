import BrowserExtension from "./BrowserExtension.js";
import { initializeGlobalVariable } from "./GlobalVariableInitializer.js";

/**
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionMode} BrowserExtensionMode
 */

/**
 * Initializes the Blazor Browser Extension internally
 * @param {object} options The initialization options.
 * @param {string} options.CompressionEnabled The project name.
 * @param {string} browserExtensionUrl The browser extension url.
 * @param {BrowserExtensionMode} browserExtensionMode The browser extension mode.
 * @returns {BrowserExtension}
 */
export function initializeInternal({ CompressionEnabled }, browserExtensionUrl, browserExtensionMode) {
  const compressionEnabled = CompressionEnabled.toLowerCase() !== "false";
  const browserExtension = new BrowserExtension(browserExtensionUrl, browserExtensionMode, compressionEnabled);
  initializeGlobalVariable(browserExtension);
  return browserExtension;
}