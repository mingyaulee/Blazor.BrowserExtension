/** @typedef {"Standard" | "ContentScript"} BrowserExtensionMode */

/**
 * @typedef BrowserExtensionModesEnum
 * @property {BrowserExtensionMode} Standard
 * @property {BrowserExtensionMode} ContentScript
 */

/**
 * @type {BrowserExtensionModesEnum}
 */
const BrowserExtensionModes = {
  Standard: "Standard",
  ContentScript: "ContentScript"
};

export default BrowserExtensionModes;