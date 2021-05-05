/** @typedef {string} BrowserExtensionMode */

/** 
 * @typedef BrowserExtensionModesEnum
 * @property {string} Standard "Standard"
 * @property {string} ContentScript "ContentScript"
 */

/**
 * @type {BrowserExtensionModesEnum}
 * @enum {BrowserExtensionMode}
 */
const BrowserExtensionModes = {
  Standard: "Standard",
  ContentScript: "ContentScript"
};

export default BrowserExtensionModes;