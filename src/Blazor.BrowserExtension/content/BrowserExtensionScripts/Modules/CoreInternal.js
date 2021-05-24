import BrowserExtension from "./BrowserExtension.js";
import { initializeGlobalVariable } from "./GlobalVariableInitializer.js";

/**
 * Initializes the Blazor Browser Extension internally
 * @param {string} projectName The project name.
 * @param {string} browserExtensionUrl The browser extension url.
 * @param {import("./BrowserExtensionModes").BrowserExtensionMode} browserExtensionMode The browser extension mode.
 * @returns {BrowserExtension}
 */
export function initializeInternal(projectName, browserExtensionUrl, browserExtensionMode) {
  // each extension's definition is stored in BlazorBrowserExtension[name]
  const browserExtension = new BrowserExtension(browserExtensionUrl, browserExtensionMode);
  initializeGlobalVariable(browserExtension.InitializeAsync);
  globalThis.BlazorBrowserExtension[projectName] = browserExtension;
  return browserExtension;
}