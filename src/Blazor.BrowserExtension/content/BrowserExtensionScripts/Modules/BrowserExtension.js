import BrowserExtensionModes, * as BrowserExtensionModesExports from "./BrowserExtensionModes.js";

export default class BrowserExtension {
  /** @type {string} */ Url;
  /** @type {BrowserExtensionModesExports.BrowserExtensionMode} */ Mode;

  /**
   * Create a new instance of BrowserExtension.
   * @param {string} url The browser extension URL.
   * @param {BrowserExtensionModesExports.BrowserExtensionMode} mode The browser extension mode.
   */
  constructor(url, mode) {
    this.Url = url;
    this.Mode = mode;
  }

  /**
   * Intercept fetch requests from blazor.webassembly.js and dotnet.*.js
   * @param {RequestInfo|string} input
   * @param {RequestInit} init?
   * @returns {Promise<Response>}
   */
  FetchAsync(input, init) {
    if (typeof (input) === "string") {
      if (input === "dotnet.wasm") {
        input = "framework/" + input;
      }
      input = this._getUrl(input);
    }
    return fetch(input, init);
  }

  /**
   * Intercept document.body.appendChild from blazor.webassembly.js and dotnet.*.js
   * @param {Element} element
   * @returns {Promise}
   */
  async AppendElementToDocumentAsync(element) {
    if (this.Mode !== BrowserExtensionModes.ContentScript) {
      /** @type {any} */(element).integrity = "";
      document.body.appendChild(element);
      return;
    }
    if (element.tagName === "SCRIPT") {
      const scriptElement = /** @type {HTMLScriptElement} */(element);
      if (scriptElement.innerText.indexOf("__wasmmodulecallback__") > -1) {
        globalThis.__wasmmodulecallback__();
        delete globalThis.__wasmmodulecallback__;
      } else if (scriptElement.src) {
        const src = this._getUrl(scriptElement.src);
        await import(src);
      } else {
        console.error("Unknown script requested", element);
        throw new Error("Unknown script requested");
      }
    } else {
      document.body.appendChild(element);
    }
  }

  /**
   * Import called from BasePage.cs
   * @param {string} script The script name to import
   * @returns {Promise}
   */
  async ImportAsync(script) {
    await import(`${this.Url}${script}`);
  }

  /**
   * Gets the URL for the path requested.
   * @param {any} path The path requested.
   * @returns {string} The absolute extension path if it is not a full URL with scheme, otherwise the original path.
   */
  _getUrl(path) {
    return path.indexOf("://") > -1 ? path : this.Url + path;
  }
}