# Background scripts and extension service worker

Different browsers have different support for [background script and extension service worker](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/manifest.json/background#browser_support).

Chrome calls it extension service worker in manifest V3 whereas Mozilla calls it background scripts, but both are declared by.the `background` key in `manifest.json`. For simplicity, we will just call them "background worker" in this page.

Background worker can be used to support the complexity in browser extensions. The pattern of content scripts, popup, or any other page communicating with the background worker to get data or instructions resembles a client-server communication.

A default background worker is created in your project.

```javascript
// Import for the side effect of defining a global 'browser' variable
import * as _ from "/content/Blazor.BrowserExtension/lib/browser-polyfill.min.js";

browser.runtime.onInstalled.addListener(() => {
  const indexPageUrl = browser.runtime.getURL("index.html");
  browser.tabs.create({
    url: indexPageUrl
  });
});
```

In the manifest, the background worker is declared in the `background` key.

```json
{
  ...
  "background": {
    "service_worker": "BackgroundWorker.js",
    "type": "module"
  },
  ...
}
```


## Mozilla Firefox

The implementation in Firefox requires a small adjustment in the `manifest.json`.

```json
{
  ...
  "background": {
    "scripts": ["BackgroundWorker.js"],
    "type": "module"
  },
  ...
}
```

## Why can't we use C# or other wasm in background worker?

The background worker is executed when the extension is first loaded and shut down when it is inactive, see [service worker lifecycle](https://developer.chrome.com/docs/extensions/develop/concepts/service-workers/lifecycle).
This requires the background worker to register all event listeners synchronously. 

Quoted from [Chrome page](https://developer.chrome.com/docs/extensions/develop/concepts/service-workers/events)

> Event handlers in service workers need to be declared in the global scope, meaning they should be at the top level of the script and not be nested inside functions. This ensures that they are registered synchronously on initial script execution, which enables Chrome to dispatch events to the service worker as soon as it starts.

Quoted from [MDN page](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Background_scripts)

> Listeners must be registered synchronously from the start of the page.
>
> Do not register listeners asynchronously, as they will not be properly triggered.
>
> Listeners must be at the top-level to activate the background script if an event is triggered. Registered listeners may need to be restructured to the synchronous pattern and moved to the top-level.

Therefore, we are unable to use wasm in the background worker, as it would require us to use the asynchronous `fetch` API to get the `.wasm` file stream to instantiate the dotnet runtime.


# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/concepts/service-workers)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Background_scripts)
