import BrowserExtension from "./BrowserExtension.js";
import * as BrowserExtensionModesExports from "./BrowserExtensionModes.js";

/**
 * @callback InitializeFunction
 * @param {string} projectName
 * @returns {Promise<BrowserExtension>}
 */

export default class BlazorBrowserExtension {
  /** @type {string} */ Url;
  /** @type {BrowserExtensionModesExports.BrowserExtensionModesEnum} */ Modes;
  /** @type {InitializeFunction} */ InitializeAsync;
}