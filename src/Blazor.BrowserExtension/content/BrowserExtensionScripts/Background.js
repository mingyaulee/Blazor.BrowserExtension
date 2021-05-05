(async () => {
  const projectName = "__ProjectName__";
  const browserExtension = globalThis.BlazorBrowserExtension[projectName];

  const prefixUri = browserExtension.Url;
  const listener = details => {
    if (details.url.startsWith(prefixUri) && !isStaticFile(details.url)) {
      const newUrl = prefixUri + "index.html?path=" + encodeURIComponent(details.url.substr(prefixUri.length));
      return {
        redirectUrl: newUrl
      };
    }
  };
  const isStaticFile = url => {
    return /\.\w{2,5}(\?.+)?$/.test(url);
  };
  var hasPermission = await globalThis.browser.permissions.contains({ permissions: ["webRequest", "webRequestBlocking"] });
  if (hasPermission) {
    globalThis.browser.webRequest.onBeforeRequest.addListener(listener, { urls: [prefixUri + "*"] }, ["blocking"]);
  }
})();