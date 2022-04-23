# Migrate from Manifest V2 to V3

1. Update the `Blazor.BrowserExtension` NuGet package version to at least `v0.10.0`.
0. Update target framework to `net7.0` and all .Net packages to .Net 7.
0. Update the `manifest.json` file
   - Update `manifest_version`
     ```json
     "manifest_version": 3,
     ```
   - Update `background`
     ```json
     "background": {
       "service_worker": "BackgroundWorker.js",
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
0. Move all the codes from `Background.razor` to `BackgroundWorker.js`
   - Create a new file `BackgroundWorker.js` in `wwwroot`.
   - Remove `Background.razor`
   - Refer to [this guide](https://developer.chrome.com/docs/extensions/mv3/migrating_to_service_workers/) for migrating from background page to background service worker.

