/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionModesEnum} BrowserExtensionModesEnum
 */

export default class BlazorBrowserExtension {
  /** @type {boolean} */
  ImportBrowserPolyfill;
  /** @type {boolean} */
  StartBlazorBrowserExtension;
  /** @type {BrowserExtensionModesEnum} */
  Modes;
  /** @type {BrowserExtension} */
  BrowserExtension;

  constructor() {
    this.ImportBrowserPolyfill = true;
    this.StartBlazorBrowserExtension = true;
    this.Modes = null;
    this.BrowserExtension = null;
  }
}