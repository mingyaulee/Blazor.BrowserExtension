/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionModesEnum} BrowserExtensionModesEnum
 */

/**
 * @callback InitializeFunction
 * @param {string} environmentName
 * @returns {Promise<BrowserExtension>}
 */

export default class BlazorBrowserExtension {
  constructor() {
    /** @type {string} */ this.Url = null;
    /** @type {BrowserExtensionModesEnum} */ this.Modes = null;
    /** @type {InitializeFunction} */ this.InitializeAsync = null;
  }
}