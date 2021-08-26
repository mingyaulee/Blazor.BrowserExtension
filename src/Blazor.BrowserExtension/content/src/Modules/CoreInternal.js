import BrowserExtension from "./BrowserExtension.js";
import { initializeGlobalVariable } from "./GlobalVariableInitializer.js";

/**
 * Initializes the Blazor Browser Extension internally
 * @param {object} options The initialization options.
 * @param {string} options.ProjectName The project name.
 * @param {string} options.CompressionEnabled The project name.
 * @param {string} browserExtensionUrl The browser extension url.
 * @param {import("./BrowserExtensionModes").BrowserExtensionMode} browserExtensionMode The browser extension mode.
 * @returns {BrowserExtension}
 */
export function initializeInternal({ ProjectName, CompressionEnabled }, browserExtensionUrl, browserExtensionMode) {
  const compressionEnabled = CompressionEnabled.toLowerCase() !== "false";
  // each extension's definition is stored in BlazorBrowserExtension[name]
  const browserExtension = new BrowserExtension(browserExtensionUrl, browserExtensionMode, compressionEnabled);
  initializeGlobalVariable(browserExtension.InitializeAsync);
  globalThis.BlazorBrowserExtension[ProjectName] = browserExtension;
  return browserExtension;
}