﻿const proxies = {
  "document.addEventListener": () => { },
  "document.childNodes": [],
  "document.createComment": () => ProxyTarget.createProxy("comment()", globalThis.Comment),
  "document.createElement": (tagName) => ProxyTarget.createProxy(`element(${tagName})`, globalThis.Element),
  "document.createElementNS": () => ProxyTarget.createProxy("elementns()", globalThis.Element),
  "document.currentScript.getAttribute": () => "false", // Called by Blazor, return false to prevent autostart
  "document.documentElement.style.setProperty": () => { },
  "document.hasChildNodes": () => false,
  "document.querySelector": () => { },
  "document.querySelectorAll": () => [],
  "window.addEventListener": () => { },
  "window.frameElement": null,
  "window.opener": null,
  "window.parent": null,
  "window.removeEventListener": () => { },
  "window.sessionStorage": {},
  "Element.appendChild": () => { },
  "Element.getRootNode": () => globalThis.document,
};

const getProxyClassSymbol = Symbol();
const getProxyTargetSymbol = Symbol();
let suppressCreateProxy = 0;

class ProxyHandlerContext {
  target;
  propertyName = "";
  proxyKey = "";
  result;
}

const symbolHandler = (/** @type {string|symbol} */ propertyNameOrSymbol, /** @type {ProxyHandlerContext} */ context) => {
  if (typeof (propertyNameOrSymbol) === "string") {
    context.propertyName = propertyNameOrSymbol;
    return false;
  }

  const symbol = propertyNameOrSymbol;

  // Handle symbols defined above
  if (symbol === getProxyClassSymbol && context.target instanceof ProxyTarget) {
    const proxyClass = context.target.getProxyClass();
    context.result = proxyClass?.[getProxyTargetSymbol] ?? proxyClass;
  } else if (symbol === getProxyTargetSymbol) {
    context.result = context.target;
  } else if (symbol.description?.startsWith("Symbol.")) {
    // For other symbols
    // if it has a description, we can proxy it
    context.propertyName = symbol.description.substring("Symbol.".length);
    return false;
  } else {
    // otherwise ignore them
    context.result = context.target[symbol];
  }

  return true;
};

const cachedValueHandler = (/** @type {ProxyHandlerContext} */ context) => {
  if (context.target.hasOwnProperty(context.propertyName)) {
    // return cached value
    context.result = context.target[context.propertyName];
    return true;
  }

  return false;
};

const specialPropertiesHandler = (/** @type {ProxyHandlerContext} */ context) => {
  if (context.target instanceof ProxyTarget) {
    if (context.target.getProxyName() === "window" && globalThis.hasOwnProperty(context.propertyName)) {
      // mirror window to globalThis
      context.result = globalThis[context.propertyName];
      return true;
    }

    if (context.propertyName === "hasInstance") {
      // support "instanceof" operation
      context.result = (obj) => obj?.[getProxyClassSymbol] === context.target;
      return true;
    }
  }

  return false;
};

const definedProxiesHandler = (/** @type {ProxyHandlerContext} */ context) => {
  context.proxyKey = context.target instanceof ProxyTarget
    ? context.target.getProxyName() + "." + context.propertyName
    : context.propertyName;

  if (proxies.hasOwnProperty(context.proxyKey)) {
    context.result = proxies[context.proxyKey];
    return true;
  }

  return false;
};

const proxyClassHandler = (/** @type {ProxyHandlerContext} */ context) => {
  if (context.target instanceof ProxyTarget) {
    // get value from base class
    const proxyClass = context.target.getProxyClass();
    if (proxyClass) {
      suppressCreateProxy++;
      const proxyClassProperty = proxyClass[context.propertyName];
      suppressCreateProxy--;
      if (proxyClassProperty) {
        context.result = proxyClassProperty;
        return true;
      }
    }
  }

  return false;
};

const handler = {
  get(target, propertyNameOrSymbol) {
    const context = new ProxyHandlerContext();
    context.target = target;

    const isHandled = symbolHandler(propertyNameOrSymbol, context) ||
      cachedValueHandler(context) ||
      specialPropertiesHandler(context) ||
      definedProxiesHandler(context) ||
      proxyClassHandler(context);

    if (!isHandled && suppressCreateProxy === 0) {
      context.result = ProxyTarget.createProxy(context.proxyKey)
      target[context.propertyName] = context.result;
    }

    return context.result;
  },

  set(target, propertyName, value) {
    target[propertyName] = value;

    if (target instanceof ProxyTarget && target.getProxyName() === "window") {
      // mirror window to globalThis
      globalThis[propertyName] = value;
    }

    return true;
  }
}

export class ProxyTarget {
  #name;
  #instanceOfClass;

  constructor(name, instanceOfClass) {
    this.#name = name;
    this.#instanceOfClass = instanceOfClass;
  }

  getProxyName() {
    return this.#name;
  }

  getProxyClass() {
    return this.#instanceOfClass;
  }

  static createProxy(name, instanceOfClass) {
    return new Proxy(new ProxyTarget(name, instanceOfClass), handler);
  }
}

export function defineProxy(key, value) {
  proxies[key] = value;
}

export const requireProxy = globalThis.window === undefined && globalThis.document == undefined;

if (requireProxy) {
  globalThis.window = ProxyTarget.createProxy("window");
  globalThis.document = ProxyTarget.createProxy("document");
  globalThis.history = ProxyTarget.createProxy("history");

  globalThis.Node = ProxyTarget.createProxy("Node");
  globalThis.Comment = ProxyTarget.createProxy("Comment");
  globalThis.Element = ProxyTarget.createProxy("Element");
  globalThis.HTMLElement = ProxyTarget.createProxy("HTMLElement ");
  globalThis.DocumentFragment = ProxyTarget.createProxy("DocumentFragment");
}
