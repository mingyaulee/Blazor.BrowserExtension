const BrowserExtensionModes = {
  Standard: "Standard",
  Background: "Background",
  ContentScript: "ContentScript",
  Debug: "Debug"
};

class BrowserExtension {
  constructor(url, mode, config) {
    this.Url = url;
    this.Mode = mode;
    this.Config = config;
  }

  async InitializeCoreAsync(blazorStartOptions) {
    if (this.Config.CompressionEnabled) {
      // import brotli decode.js
      this.BrotliDecode = (await import('./lib/decode.min.js')).BrotliDecode;
    }

    // Workaround for https://github.com/dotnet/aspnetcore/issues/54358
    // Import dotnet to change the environment and boot resource loader
    const { dotnet } = await import(`${this.Url}_framework/dotnet.js`);

    if (this.Config.EnvironmentName && !blazorStartOptions.environment) {
      blazorStartOptions.environment = this.Config.EnvironmentName;
      dotnet.withApplicationEnvironment(this.Config.EnvironmentName);
    }

    if (this.Mode === BrowserExtensionModes.ContentScript || this.Config.CompressionEnabled) {
      blazorStartOptions.loadBootResource = this._loadBootResource.bind(this);
      dotnet.withResourceLoader(this._loadBootResource.bind(this));
    }
  }

  async InitializeContentScriptAsync(blazorStartOptions) {
    await this.InitializeCoreAsync(blazorStartOptions);
    await this._startBlazor(blazorStartOptions);
  }

  async InitializeAsync(blazorStartOptions) {
    if (!blazorStartOptions) {
      blazorStartOptions = {};
    }

    await this.InitializeCoreAsync(blazorStartOptions);
    await this._startBlazor(blazorStartOptions);

    return this;
  }

  FetchAsync(input, init) {
    if (typeof (input) === "string") {
      if (input === "dotnet.wasm") {
        input = "_framework/" + input;
      }
      input = this._getUrl(input);
    }
    return fetch(input, init);
  }

  async AppendElementToDocumentAsync(element) {
    if (element.tagName === "SCRIPT") {
      const scriptElement = (element);
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
      const linkElement = (element);
      if (linkElement.rel == "modulepreload") ; else {
        await this._appendElementToDocumentAsync(element);
      }
    } else {
      await this._appendElementToDocumentAsync(element);
    }
  }

  async ImportAsync(script) {
    await import(`${this.Url}${script}`);
  }

  _getUrl(path) {
    return path.indexOf("://") > -1 ? path : this.Url + path;
  }

  async _appendElementToDocumentAsync(element) {
    return new Promise(resolve => {
      let immediateResolve = true;
      if (element.tagName === "SCRIPT") {
        immediateResolve = false;
        (element).onload = () => resolve();
      }
      document.body.appendChild(element);
      if (immediateResolve) {
        resolve();
      }
    });
  }

  async _startBlazor(blazorStartOptions) {
    // import blazor.webassembly.js
    const blazorScript = globalThis.document.createElement("script");
    blazorScript.src = `${this.Url}_framework/blazor.webassembly.js`;
    blazorScript.defer = true;
    // Blazor is set to not auto start, so that we can start it with different start options
    blazorScript.setAttribute("autostart", "false");
    await this.AppendElementToDocumentAsync(blazorScript);

    // Start Blazor
    const blazorStart = globalThis.Blazor.start(blazorStartOptions);
    if (blazorStart && blazorStart instanceof Promise) {
      await blazorStart;
    }
  }

  _loadBootResource(resourceType, resourceName, defaultUri, _integrity) {
    if (resourceType === "dotnetjs" || resourceType === "manifest" || resourceType === "configuration") {
      return `${this.Url}_framework/${resourceName}`;
    }

    if (this.Config.CompressionEnabled) {
      return (async () => {
        const response = await this.FetchAsync(defaultUri + ".br", { cache: "no-cache" });
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

  _getBrowserExtensionEnvironment() {
    return `${this.Mode}|${this.Url}`;
  }

  _getBrowserExtensionModeLegacy() {
    return globalThis.BINDING.js_string_to_mono_string(this.Mode);
  }
}

class BlazorBrowserExtension {
  ImportBrowserPolyfill;
  StartBlazorBrowserExtension;
  Modes;
  BrowserExtension;

  constructor() {
    this.ImportBrowserPolyfill = true;
    this.StartBlazorBrowserExtension = true;
    this.Modes = null;
    this.BrowserExtension = null;
  }
}

function initializeGlobalVariable(browserExtension) {
  let blazorBrowserExtension;

  // initialize global property BlazorBrowserExtension
  if (!globalThis.hasOwnProperty("BlazorBrowserExtension")) {
    blazorBrowserExtension = new BlazorBrowserExtension();
    blazorBrowserExtension.Modes = BrowserExtensionModes;
    globalThis.BlazorBrowserExtension = blazorBrowserExtension;
  } else {
    blazorBrowserExtension = (globalThis.BlazorBrowserExtension);
  }

  if (blazorBrowserExtension.BrowserExtension) {
    // Extensions execution should be isolated so this property should be null upon initialization.
    throw new Error("Browser extension cannot be loaded.");
  }

  blazorBrowserExtension.BrowserExtension = browserExtension;

  return blazorBrowserExtension;
}

function initializeInternal(config, browserExtensionUrl, browserExtensionMode) {
  const browserExtension = new BrowserExtension(browserExtensionUrl, browserExtensionMode, config);
  return initializeGlobalVariable(browserExtension);
}

export { initializeInternal };
