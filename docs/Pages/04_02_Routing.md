# How does routing work?

Physical file routing is used to serve different pages.

When building the project:

1. All the Razor components are processed to get a list of all the physical file routes from the `@page` attribute.
0. The routing entry file (default is `index.html`, see [`BrowserExtensionRoutingEntryFile`](04_01_ConfigureBuild.md)) is replicated in the output directory based on the list of physical file routes.

For example, if the `Popup.razor` contains `@page "/popup.html"` and the `Options.razor` contains `@page "/options.html"`, when the project is built or published, the file `index.html` will be replicated in the output directory with the names `popup.html` and `options.html`.

This is especially useful for browser extensions because the browsers only serve static files, and the presence of these physical files supports the routing when the extension page is reloaded.
