/** @typedef {"Standard" | "Background" | "ContentScript" | "Debug"} BrowserExtensionMode */

/**
 * @typedef BrowserExtensionModesEnum
 * @property {BrowserExtensionMode} Standard
 * @property {BrowserExtensionMode} Background
 * @property {BrowserExtensionMode} ContentScript
 * @property {BrowserExtensionMode} Debug
 */

/**
 * @type {BrowserExtensionModesEnum}
 */
const BrowserExtensionModes = {
  Standard: "Standard",
  Background: "Background",
  ContentScript: "ContentScript",
  Debug: "Debug"
};

export default BrowserExtensionModes;