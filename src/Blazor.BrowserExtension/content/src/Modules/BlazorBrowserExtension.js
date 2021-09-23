/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionModesEnum} BrowserExtensionModesEnum
 */

export default class BlazorBrowserExtension {
  constructor() {
    /** @type {boolean} */ this.ImportBrowserPolyfill = true;
    /** @type {boolean} */ this.StartBlazorBrowserExtension = true;
    /** @type {BrowserExtensionModesEnum} */ this.Modes = null;
    /** @type {BrowserExtension} */ this.BrowserExtension = null;
  }
}