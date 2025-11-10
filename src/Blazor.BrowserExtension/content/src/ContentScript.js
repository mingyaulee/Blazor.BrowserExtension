(async () => {
  const url = (globalThis.browser || globalThis.chrome).runtime.getURL("");
  const initializeInternal = (await import("./Modules/CoreInternal.js")).initializeInternal;
  const documentBaseUriWithoutPath = new URL("/", document.baseURI).toString();

  const configRequest = await fetch(`${url}content/browserextension.config.json`);
  /** @type {import("./Modules/BrowserExtensionConfig.js").default} */
  const config = await configRequest.json();

  if (typeof globalThis.cloneInto === "function") {
    // In dotnet.js the blazor.boot.json is fetched and the properties in the response JSON object is updated
    // Firefox requires objects to be cloned into the content script context because fetch is considered priviledged access API
    // Error: Not allowed to define cross-origin object as property on [Object] or [Array] XrayWrapper
    const fetchFunction = globalThis.fetch;
    globalThis.fetch = async (/** @type string */fetchUrl, init) => {
      if (typeof fetchUrl !== "string" || !fetchUrl.includes("blazor.boot.json")) {
        return fetchFunction(fetchUrl, init);
      }
      const absoluteFetchUrl = new URL(fetchUrl, url);
      const response = await fetchFunction(absoluteFetchUrl, init);
      const proxyHandler = {
        get(target, prop) {
          if (prop === "json") {
            return async () => {
              const json = await response.json();
              return globalThis.cloneInto(json, globalThis);
            };
          }

          return target[prop];
        }
      };

      return new Proxy(response, proxyHandler);
    };
  }

  const blazorBrowserExtension = initializeInternal(config, url, "ContentScript");

  globalThis.importProxy = (requestorRelativePath, module) => {
    if (module.startsWith(document.baseURI)) {
      module = new URL(module.substring(document.baseURI.length), url);
    } else if (module.startsWith(documentBaseUriWithoutPath)) {
      module = new URL(module.substring(documentBaseUriWithoutPath.length), url);
    } else if (module.startsWith(".")) {
      module = new URL(`${requestorRelativePath}/${module}`, url);
    }

    return import(module);
  };

  await blazorBrowserExtension.BrowserExtension.InitializeContentScriptAsync({
    BlazorBrowserExtension: blazorBrowserExtension
  });
})();
