(function (global, projectName) {
    const browserExtensionModes = {
        Standard: "Standard",
        ContentScript: "ContentScript"
    };

    class BrowserExtension {
        /** @type {string} */ Url;
        /** @type {string} */ Mode;

        /**
         * Create a new instance of BrowserExtension.
         * @param {string} url The browser extension URL.
         * @param {string} mode The browser extension mode.
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
         */
        async AppendElementToDocumentAsync(element) {
            if (this.Mode !== browserExtensionModes.ContentScript) {
                /** @type {any} */(element).integrity = "";
                document.body.appendChild(element);
                return;
            }
            if (element.tagName === "SCRIPT") {
                const scriptElement = /** @type {HTMLScriptElement} */(element);
                if (scriptElement.innerText.indexOf("__wasmmodulecallback__") > -1) {
                    global.__wasmmodulecallback__();
                    delete global.__wasmmodulecallback__;
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

    /**
     * Gets the browser extension mode.
     * @param {string} extensionUrl The extension URL.
     */
    function getBrowserExtensionMode(extensionUrl) {
        // Set browser extension mode to ContentScript for URL that does not match the browser extension URL
        if (extensionUrl.startsWith(global.location.origin)) {
            return browserExtensionModes.Standard;
        } else {
            return browserExtensionModes.ContentScript;
        }
    }

    /**
     * Initializes the Blazor Browser Extension application
     * @returns {Promise}
     */
    async function initialize() {
        // import browser extension API polyfill
        // @ts-ignore JS is not a module
        await import("./lib/browser-polyfill.min.js");

        // each extension's definition is stored in BlazorBrowserExtension[name]
        const url = global.browser.runtime.getURL("");
        const browserExtension = new BrowserExtension(url, getBrowserExtensionMode(url));
        global.BlazorBrowserExtension[projectName] = browserExtension;

        // import WebExtension.Net JS
        await import(`${url}WebExtensionScripts/WebExtensionNet.js`);

        // import blazor.webassembly.js
        const blazorScript = document.createElement("script");
        blazorScript.src = `${url}framework/blazor.webassembly.js`;
        blazorScript.defer = true;
        // Blazor is set to not auto start, so that we can start it with different environment name
        blazorScript.setAttribute("autostart", "false");
        await browserExtension.AppendElementToDocumentAsync(blazorScript);

        // Start Blazor
        if (browserExtension.Mode === browserExtensionModes.Standard) {
            // Blazor script tag is injected, run start after the script is loaded
            blazorScript.onload = () => {
                global.Blazor.start({
                    environment: browserExtension.Mode
                });
            }
        } else {
            // Blazor is imported, we can start it rigt away
            global.Blazor.start({
                environment: browserExtension.Mode,
                loadBootResource: function (resourceType, resourceName, defaultUri, integrity) {
                    if (resourceType === "dotnetjs") {
                        return `${url}framework/${resourceName}`;
                    }
                    return defaultUri;
                }
            });
        }
    }

    // initialize global property BlazorBrowserExtension
    if (!global.hasOwnProperty("BlazorBrowserExtension")) {
        global.BlazorBrowserExtension = {
        };
    }

    if (!global.BlazorBrowserExtension.hasOwnProperty("")) {
        global.BlazorBrowserExtension.Modes = browserExtensionModes;
    }

    global.BlazorBrowserExtension.Initialize = initialize;

    if (global.StartBlazorBrowserExtension !== false) {
        initialize();
    }
})(globalThis, "__ProjectName__");