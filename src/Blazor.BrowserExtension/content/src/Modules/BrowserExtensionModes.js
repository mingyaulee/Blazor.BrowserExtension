/** @typedef {"Standard" | "ContentScript" | "Debug"} BrowserExtensionMode */

/**
 * @typedef BrowserExtensionModesEnum
 * @property {BrowserExtensionMode} Standard
 * @property {BrowserExtensionMode} ContentScript
 * @property {BrowserExtensionMode} Debug
 */

/**
 * @type {BrowserExtensionModesEnum}
 */
const BrowserExtensionModes = {
  Standard: "Standard",
  ContentScript: "ContentScript",
  Debug: "Debug"
};

export default BrowserExtensionModes;