import BrowserExtensionModes from "./BrowserExtensionModes.js";

/**
 * @typedef {import("./BrowserExtensionModes").BrowserExtensionMode} BrowserExtensionMode
 * @typedef {import("./BrowserExtensionConfig.js").default} BrowserExtensionConfig
 */

export default class BrowserExtension {
  /**
   * Create a new instance of BrowserExtension.
   * @param {string} url The browser extension URL.
   * @param {BrowserExtensionMode} mode The browser extension mode.
   * @param {BrowserExtensionConfig} config Indicate if compression is enabled.
   */
  constructor(url, mode, config) {
    this.Url = url;
    this.Mode = mode;
    this.Config = config;
  }

  /**
   * Initializes the browser extension.
   * @param {object} blazorStartOptions Blazor WebAssembly start options. Refer to https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Platform/WebAssemblyStartOptions.ts
   * @returns {Promise<BrowserExtension>}
   */
  async InitializeAsync(blazorStartOptions) {
    // import JsBind.Net JS
    await import(`${this.Url}content/JsBind.Net/JsBindNet.js`);

    if (this.Config.CompressionEnabled) {
      // import brotli decode.js
      this.BrotliDecode = (await import("../lib/decode.min.js")).BrotliDecode;
    }

    // import blazor.webassembly.js
    const blazorScript = globalThis.document.createElement("script");
    blazorScript.src = `${this.Url}framework/blazor.webassembly.js`;
    blazorScript.defer = true;
    // Blazor is set to not auto start, so that we can start it with different start options
    blazorScript.setAttribute("autostart", "false");
    await this.AppendElementToDocumentAsync(blazorScript);

    // Start Blazor
    if (this.Config.EnvironmentName && !blazorStartOptions.environment) {
      blazorStartOptions.environment = this.Config.EnvironmentName;
    }

    if (this.Mode === BrowserExtensionModes.ContentScript || this.Config.CompressionEnabled) {
      blazorStartOptions.loadBootResource = this._loadBootResource.bind(this);
    }
    const blazorStart = globalThis.Blazor.start(blazorStartOptions);
    if (blazorStart && blazorStart instanceof Promise) {
      await blazorStart;
    }
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
    if (element.tagName === "SCRIPT") {
      const scriptElement = /** @type {HTMLScriptElement} */(element);
      if (scriptElement.text.indexOf("__wasmmodulecallback__") > -1) {
        globalThis.__wasmmodulecallback__();
        delete globalThis.__wasmmodulecallback__;
      } else if (scriptElement.src) {
        const src = this._getUrl(scriptElement.src);
        await import(src);
      } else {
        console.error("Unknown script requested", element);
        throw new Error("Unknown script requested");
      }
    } else if (element.tagName == "LINK") {
      const linkElement = /** @type {HTMLLinkElement} */(element);
      if (linkElement.rel == "modulepreload") {
        // do nothing because we do not need to preload
      } else {
        await this._appendElementToDocumentAsync(element);
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
   * Appends element to document.
   * @param {Element} element
   * @returns {Promise}
   */
  async _appendElementToDocumentAsync(element) {
    return new Promise(resolve => {
      let immediateResolve = true;
      if (element.tagName === "SCRIPT") {
        immediateResolve = false;
        /** @type {HTMLScriptElement} */(element).onload = () => resolve();
      }
      document.body.appendChild(element);
      if (immediateResolve) {
        resolve();
      }
    });
  }

  /**
   * Loads boot resource for Blazor application.
   * @param {any} resourceType
   * @param {any} resourceName
   * @param {any} defaultUri
   * @param {any} _integrity
   */
  _loadBootResource(resourceType, resourceName, defaultUri, _integrity) {
    if (resourceType === "dotnetjs" || resourceType === "manifest") {
      return `${this.Url}framework/${resourceName}`;
    }

    if (this.Config.CompressionEnabled) {
      return (async () => {
        const response = await this.FetchAsync(defaultUri + '.br', { cache: 'no-cache' });
        if (!response.ok) {
          throw new Error(response.statusText);
        }
        const originalResponseBuffer = await response.arrayBuffer();
        const originalResponseArray = new Int8Array(originalResponseBuffer);
        const decompressedResponseArray = this.BrotliDecode(originalResponseArray);
        const contentType = resourceType === "dotnetwasm" ? "application/wasm" : "application/octet-stream";
        return new Response(decompressedResponseArray, { headers: { "content-type": contentType } });
      })();
    }

    return defaultUri;
  }

  /**
   * Gets the browser extension mode. Called during initialization in DotNet.
   * @returns {any}
   */
  _getBrowserExtensionMode() {
    return this.Mode;
  }

  /**
   * Gets the browser extension mode. Called during initialization in DotNet.
   * @returns {any}
   */
  _getBrowserExtensionModeLegacy() {
    return globalThis.BINDING.js_string_to_mono_string(this.Mode);
  }
}