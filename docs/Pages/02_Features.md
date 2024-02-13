# Features

- This package imports three other packages, which are:
  1. [WebExtensions.Net](https://github.com/mingyaulee/WebExtensions.Net) - Provides interop for WebExtensions standard API.
  2. [JsBind.Net](https://github.com/mingyaulee/JsBind.Net) - Provides advanced JavaScript interop features used by WebExtensions.Net.
  3. Blazor.BrowserExtension.Build (in this repository) - Adds build targets and tasks to the project.

- Hook into Blazor lifecycle events with [`app.js`](03_01_app.js.md).
- Publishing with Brotli compression enabled by default to minimize published extension app size.

- Prepares the extension files when building and publishing.
  - Replicates index.html to enable [routing](04_02_Routing.md).
  - Copies all static web assets from other RCLs.

- Configurable build and publish with [MSBuild properties](04_01_ConfigureBuild.md).

## Samples/References

Sample projects are available in the repository [Blazor.BrowserExtension.Samples](https://github.com/mingyaulee/Blazor.BrowserExtension.Samples).

You can also refer to the following projects for real life extensions:

- [Blazor Edge New Tab](https://github.com/dragnilar/EdgeExtensionsBlazor) - Published for [Chrome](https://chrome.google.com/webstore/detail/blazor-edge-new-tab/bdcfngldhocoffghnlmhibpifmoakiec) and [Edge](https://microsoftedge.microsoft.com/addons/detail/blazor-edge-new-tab/bfhdepjammnaoddhikhogfbnikmeocfj).
- [Amazing Favorites](https://github.com/Amazing-Favorites/Amazing-Favorites) - Published for [Chrome](https://chrome.google.com/webstore/detail/amazing-favorites/podhpclhgkdeiechkdceginfehfanhcb) and [Edge](https://microsoftedge.microsoft.com/addons/detail/amazing-favorites/bknjgbpkaloajcphccpcnahegfglfiei).

Or check out the [GitHub dependency graph](https://github.com/mingyaulee/Blazor.BrowserExtension/network/dependents) for more repositories.

> If you want to add another project to the list, create a PR or issue in GitHub.


## More samples

There are plenty more samples provided by Chrome and MDN in vanilla JavaScript, which of course in most cases can be ported to Blazor.

Check out these repositories to get inspired:

1. [webextensions-examples](https://github.com/mdn/webextensions-examples)
2. [chrome-extensions-samples](https://github.com/GoogleChrome/chrome-extensions-samples)
