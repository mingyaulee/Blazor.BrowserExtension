# Manifest V3 is supported

What is Manifest V3? Read more about manifest V3 [here](https://developer.chrome.com/docs/extensions/develop/migrate/what-is-mv3).

At the moment, Chromium based browsers (Chrome & Edge) have support for the manifest V3 specification and manifest V2 is deprecated.

Firefox's implementation has slight differences with Chromium based browsers in the [manifest format](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/manifest.json).


## Migrate from Manifest V2 to V3

1. Update the `Blazor.BrowserExtension` NuGet package version to the latest version.
0. Update target framework to `net9.0` and all .Net packages to .Net 9.
0. Update the `manifest.json` file
   - Update `manifest_version`
     ```json
     "manifest_version": 3,
     ```
   - Update `background`
     ```json
     "background": {
       "service_worker": "content/BackgroundWorker.js",
       "type": "module"
     },
     ```
   - Update `browser_action`
     ```json
     "action": {
       "default_popup": "popup.html"
     },
     ```
   - Update `content_security_policy`
     ```json
     "content_security_policy": {
       "extension_pages": "script-src 'self' 'wasm-unsafe-eval'; object-src 'self'"
     }
     ```
   - Update `web_accessible_resources`
     ```json
     "web_accessible_resources": [
       {
         "resources": [
           ...
         ],
         "matches": [ "<all_urls>" ]
       }
     ]
     ```
0. Move all the codes from `Background.razor` to `BackgroundWorker.cs`
   - Create a new file `BackgroundWorker.cs`.
   - Refer to the [Background Worker page](03_02_BackgroundWorker.md) for the sample class.
   - Remove `Background.razor`
   - Refer to [this guide](https://developer.chrome.com/docs/extensions/mv3/migrating_to_service_workers/) for migrating from background page to background service worker.


# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/migrate)
