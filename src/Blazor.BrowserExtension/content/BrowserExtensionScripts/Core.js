(async function (global, name) {
    // import browser extension API polyfill
    await import("./lib/browser-polyfill.min.js");

    const browserExtensionModes = {
        Standard: "Standard",
        ContentScript: "ContentScript"
    };

    // each extension's definition is stored in BlazorBrowserExtension[name]
    if (!global.hasOwnProperty("BlazorBrowserExtension")) {
        global.BlazorBrowserExtension = {
            Modes: browserExtensionModes
        };
    }

    const url = browser.runtime.getURL("");
    const getUrl = path => {
        return path.indexOf("://") > -1 ? path : url + path;
    };
    const browserExtension = {
        Mode: "",
        URL: url,
        FetchAsync: function (input, init) {
            // Intercept fetch requests from blazor.webassembly.js and dotnet.*.js
            if (typeof (input) === "string") {
                if (input === "dotnet.wasm") {
                    input = "framework/" + input;
                }
                input = getUrl(input);
            }
            return fetch(input, init);
        },
        ImportAsync: async function (script) {
            // Import called from BasePage.cs
            await import(`${url}${script}`);
        },
        AppendElementToDocumentAsync: async function (element) {
            // Intercept document.body.appendChild from blazor.webassembly.js and dotnet.*.js
            if (this.Mode !== browserExtensionModes.ContentScript) {
                element.integrity = "";
                document.body.appendChild(element);
                return;
            }
            if (element.tagName === "SCRIPT") {
                if (element.innerText.indexOf("__wasmmodulecallback__") > -1) {
                    global.__wasmmodulecallback__();
                    delete global.__wasmmodulecallback__;
                } else if (element.src) {
                    const src = getUrl(element.src);
                    await import(src);
                } else {
                    console.error("Unknown script requested", element);
                    throw new Error("Unknown script requested");
                }
            } else {
                document.body.appendChild(element);
            }
        }
    };
    global.BlazorBrowserExtension[name] = browserExtension;

    // import WebExtension.Net JS
    await import(`${url}WebExtensionScripts/WebExtensionNet.js`);

    // Set browser extension mode to ContentScript for URL that does not match the browser extension URL
    if (url.startsWith(global.location.origin)) {
        browserExtension.Mode = browserExtensionModes.Standard;
    } else {
        browserExtension.Mode = browserExtensionModes.ContentScript;
    }

    // Import and execute blazor.webassembly.js
    const blazorScript = document.createElement("script");
    blazorScript.src = `${url}framework/blazor.webassembly.js`;
    blazorScript.defer = true;
    blazorScript.setAttribute("autostart", "false");
    await browserExtension.AppendElementToDocumentAsync(blazorScript);

    // Blazor is set to not auto start, so that we can start it with different environment name
    if (browserExtension.Mode === browserExtensionModes.Standard) {
        blazorScript.onload = () => {
            Blazor.start({
                environment: browserExtension.Mode
            });
        }
    } else {
        Blazor.start({
            environment: browserExtension.Mode,
            loadBootResource: function (resourceType, name, defaultUri, integrity) {
                if (resourceType === "dotnetjs") {
                    return `${url}framework/${name}`;
                }
                return defaultUri;
            }
        });
    };
})(globalThis, "__ProjectName__");