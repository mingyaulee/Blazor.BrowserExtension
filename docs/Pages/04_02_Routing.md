# How does routing work?

## Default routing

The default routing when using this package is physical file routing.

When building the project:

1. All the Razor components are processed to get a list of all the physical file routes from the `@page` attribute.
0. The routing entry file (default is `index.html`, see [`BrowserExtensionRoutingEntryFile`](04_01_ConfigureBuild.md)) is replicated in the output directory based on the list of physical file routes.

For example, if the `Background.razor` contains `@page "/background.html"` and the `Options.razor` contains `@page "/options.html"`, when the project is built or published, the file `index.html` will be replicated in the output directory with the names `background.html` and `options.html`.

This is especially useful for browser extensions because the browsers only serve static files, and the presence of these physical files supports the routing when the extension page is reloaded.


## Virtual path routing

Routing with virtual path is also supported (**only in manifest V2**), however it is not encouraged due to the requirements.

Virtual path routing means that the routes do not have a corresponding physical file, for example `/background` or `/options`.
What this means is that when the user tries to reload the page, the browser will return a page not found (404) error.

To overcome this, the background page intercepts every requests that is made to the extension's path, and if any path does not have an extension, it will redirect the request to `index.html?path={original path}`.
This forces the browser to load the `index.html` and the `IndexPage` class uses the `NavigationManager` in Blazor to redirect to the original path.

You can use the `@page` directive to add route attribute with a virtual path to a Razor page, for example `@page "/options"`.

**Requirements:**
1. `Background.razor` must inherit from the `BackgroundPage` class.
0. `Index.razor` must inherit from the `IndexPage` class.
0. The `manifest.json` must declare the background page with `persistent: true` and the following permissions
   - `*://*/*`
   - `webRequest`
   - `webRequestBlocking`
   
   Example:
   ```json
   "permissions": [
     "*://*/*",
     "webRequest",
     "webRequestBlocking"
   ]
   ```

> **Important Note**
>
> The background will only intercept the calls if it detects the permissions are declared.
> When the permissions are not declared in the manifest, the only routing that works is the physical file routing.
