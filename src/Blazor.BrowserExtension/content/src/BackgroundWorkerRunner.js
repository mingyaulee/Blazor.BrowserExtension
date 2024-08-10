import { ProxyTarget, defineProxy, requireProxy } from "./Modules/BackgroundProxies.js";
import { initializeInternal } from "./Modules/CoreInternal.js";

let initializePromise;
let backgroundWorkerInstance;

async function initializeAsync() {
  // Initialize elements
  const elements = {
    "#background": ProxyTarget.createProxy("element(#background)", globalThis.Element),
    "#blazor-error-ui": ProxyTarget.createProxy("element(#blazor-error-ui)", globalThis.Element)
  };
  if (requireProxy) {
    globalThis.document.querySelector = (selector) => {
      if (!elements.hasOwnProperty(selector)) {
        console.error("Unexpected selector " + selector);
        elements[selector] = ProxyTarget.createProxy(`element(${selector})`, globalThis.Element);
      }

      return elements[selector];
    }

    defineProxy("element(#background).childNodes", []);
    defineProxy("element(template).content.querySelectorAll", () => []);
    defineProxy("element(template).content.firstChild", undefined);
  } else {
    const backgroundElement = globalThis.document.createElement("div");
    backgroundElement.id = "background";
    globalThis.document.body.appendChild(backgroundElement);
  }

  const url = (globalThis.browser || globalThis.chrome).runtime.getURL("");
  if (requireProxy) {
    // @ts-ignore assign the baseURI to document proxy
    globalThis.document.baseURI = url;
    // @ts-ignore assign the activeElement to document proxy
    globalThis.document.activeElement = elements["#background"];
  }

  const configRequest = await fetch(`${url}content/browserextension.config.json`);
  /** @type {import("./Modules/BrowserExtensionConfig.js").default} */
  const config = await configRequest.json();

  const blazorBrowserExtension = initializeInternal(config, url, "Background");
  const blazorStartOptions = {
    BlazorBrowserExtension: blazorBrowserExtension
  };
  await blazorBrowserExtension.BrowserExtension.InitializeCoreAsync(blazorStartOptions);

  let resolvePromise;
  const waitForBackgroundWorkerInstance = new Promise(resolve => {
    resolvePromise = resolve;
  });

  globalThis.setBackgroundWorkerInstance = (instance) => {
    backgroundWorkerInstance = instance;
    resolvePromise();
  };
  await globalThis.Blazor.start(blazorStartOptions);
  await waitForBackgroundWorkerInstance;
}

export async function runAsync(name, ...args) {
  initializePromise ??= initializeAsync();
  await initializePromise;
  const returnValue = backgroundWorkerInstance[name](...args);
  if (returnValue instanceof Promise) {
    return await returnValue;
  }
  return returnValue;
};

export function fromReference(key) {
  if (backgroundWorkerInstance) {
    return backgroundWorkerInstance[key];
  }

  return runAsync.bind(null, key);
}

export const importRequested = [];

globalThis.importProxy = (module) => {
  return new Promise(resolve => {
    importRequested.push({ module, resolve });
  });
};