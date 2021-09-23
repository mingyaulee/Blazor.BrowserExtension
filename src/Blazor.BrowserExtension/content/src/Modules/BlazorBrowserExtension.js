/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionModesEnum} BrowserExtensionModesEnum
 */

export default class BlazorBrowserExtension {
  constructor() {
    this.ImportBrowserPolyfill = true;
    this.StartBlazorBrowserExtension = true;
    /** @type {BrowserExtensionModesEnum} */
    this.Modes = null;
    /** @type {BrowserExtension} */
    this.BrowserExtension = null;
  }
}