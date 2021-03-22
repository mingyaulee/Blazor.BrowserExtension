(function (browserExtension) {
    const prefixUri = browserExtension.URL;
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
    browser.webRequest.onBeforeRequest.addListener(listener, { urls: [prefixUri + "*"] }, ["blocking"]);

})(globalThis.BlazorBrowserExtension["__ProjectName__"]);