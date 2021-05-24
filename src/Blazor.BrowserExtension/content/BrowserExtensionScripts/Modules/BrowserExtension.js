import BrowserExtensionModes from "./BrowserExtensionModes.js";

export default class BrowserExtension {
  /** @type {string} */ Url;
  /** @type {import("./BrowserExtensionModes").BrowserExtensionMode} */ Mode;

  /**
   * Create a new instance of BrowserExtension.
   * @param {string} url The browser extension URL.
   * @param {import("./BrowserExtensionModes").BrowserExtensionMode} mode The browser extension mode.
   */
  constructor(url, mode) {
    this.Url = url;
    this.Mode = mode;
  }

  /**
   * Initializes the browser extension.
   * @param {string} environment Environment name
   * @returns {Promise<BrowserExtension>}
   */
  async InitializeAsync(environment) {
    // import WebExtension.Net JS
    await import(`${this.Url}WebExtensionScripts/WebExtensionNet.js`);

    // import blazor.webassembly.js
    const blazorScript = globalThis.document.createElement("script");
    blazorScript.src = `${this.Url}framework/blazor.webassembly.js`;
    blazorScript.defer = true;
    // Blazor is set to not auto start, so that we can start it with different environment name
    blazorScript.setAttribute("autostart", "false");
    await this.AppendElementToDocumentAsync(blazorScript);

    // Start Blazor
    var startOption = {
    };

    if (environment) {
      startOption.environment = environment;
    }

    if (this.Mode === BrowserExtensionModes.ContentScript) {
      startOption.loadBootResource = (resourceType, resourceName, defaultUri, integrity) => {
        if (resourceType === "dotnetjs") {
          return `${this.Url}framework/${resourceName}`;
        }
        return defaultUri;
      };
    }
    globalThis.Blazor.start(startOption);

    return this;
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
      await this._appendElementToDocumentAsync(element);
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
      await this._appendElementToDocumentAsync(element);
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

  /**
   * Appends element fo document
   * @param {Element} element
   * @returns {Promise}
   */
  async _appendElementToDocumentAsync(element) {
    return new Promise(resolve => {
      let immediateResolve = true;
      if (element.tagName === "SCRIPT") {
        immediateResolve = false;
        /** @type {HTMLScriptElement} */ (element).onload = () => resolve();
      }
      document.body.appendChild(element);
      if (immediateResolve) {
        resolve();
      }
    });
  }

  _getBrowserExtensionMode() {
    return globalThis.BINDING.js_string_to_mono_string(this.Mode);
  }
}