/**
 * @typedef {import("./BrowserExtension.js").default} BrowserExtension
 * @typedef {import("./BrowserExtensionModes.js").BrowserExtensionModesEnum} BrowserExtensionModesEnum
 */

export default class BlazorBrowserExtension {
  /** @type {boolean} */
  ImportBrowserPolyfill = true;

  /**
   * @type {boolean}
   * @deprecated
   */
  StartBlazorBrowserExtension = true;

  /** @type {BrowserExtensionModesEnum} */
  Modes = null;

  /** @type {BrowserExtension} */
  BrowserExtension = null;
}