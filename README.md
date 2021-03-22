# Blazor.BrowserExtension
Build a browser extension with Blazor.

This package imports two other packages, which are:
1. [WebExtension.Net](https://github.com/mingyaulee/WebExtension.Net) - Provides interop for WebExtension standard API.
2. Blazor.BrowserExtension.Build (in this repository) - Adds build target and tasks to the project.

## How to use this package
1. Create a new **Blazor WebAssembly App** project (skip to step 3 if you have an existing Blazor WebAssembly project).
2. Target Framework should be at least .Net 5.0 and uncheck/deselect other options such as authentication, HTTPS support, ASP.Net Core hosting support and PWA.
3. Install NuGet package `Blazor.BrowserExtension`
4. Add a new file `manifest.json` under the `wwwroot` folder. An example of minimal `manifest.json` file:
```json
{
  "manifest_version": 2,
  "name": "My Blazor Extension",
  "description": "My browser extension built with Blazor",
  "version": "1.0",
  "background": {
    "page": "index.html?path=background",
    "persistent": true
  },
  "content_security_policy": "script-src 'self' 'unsafe-eval' 'wasm-eval' 'sha256-v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA='; object-src 'self'",
  "web_accessible_resources": [
    "framework/*",
    "BrowserExtensionScripts/*",
    "WebExtensionScripts/*"
  ],
  "permissions": [
    "*://*/*",
    "webRequest",
    "webRequestBlocking"
  ]
}
```
5. Add the following to the `.csproj` file to make sure that all the files under `wwwroot` will always be copied to the output.
```xml
  <ItemGroup>
    <None Include="wwwroot\**\*" CopyToOutputDirectory="Always" />
  </ItemGroup>
```
6. In `wwwroot/index.html` replace the script tag `<script src="_framework/blazor.webassembly.js"></script>` with `<script src="BrowserExtensionScripts/Core.js"></script>`
7. In `Pages/Index.razor` replace the first line `@page "/"` with the following lines:
```razor
@page "/"
@page "/index.html"
@inherits Blazor.BrowserExtension.Pages.IndexPage
```
8. Add a `Background.razor` file under `Pages` folder (Right click on the `Pages` folder and select Add -> Razor Component), with the following content:
```razor
@page "/background"
@inherits Blazor.BrowserExtension.Pages.BackgroundPage
```
9. Add the following into `Program.cs` file.
```csharp
using Blazor.BrowserExtension;
...
public static async Task Main(string[] args)
{
    ...
    builder.Services.AddBrowserExtensionServices(options =>
    {
        options.ProjectNamespace = typeof(Program).Namespace;
    });
    ...
}
```

## Build and load extension
In Google Chrome, go to the Extensions page (Menu -> More tools -> Extensions) and switch on Developer mode.
Click on `Load unpacked` button and navigate to `%ProjectDir%\bin\Debug\net5.0\` and select the foler `wwwroot`

## Browser Extension features

### Add a browser action popup page
Add the following to the `manifest.json`
```json
"browser_action": {
  "default_popup": "index.html?path=popup"
}
```
Add a `Popup.razor` Razor component under `Pages` folder with the following content.
```razor
@page "/popup"
@inherits Blazor.BrowserExtension.Pages.BasePage

<h1>My popup page</h1>
```

### Add an extension options page
Add the following to the `manifest.json`
```json
"options_page": "index.html?path=options"
```
Add a `Options.razor` Razor component under `Pages` folder with the following content.
```razor
@page "/options"
@inherits Blazor.BrowserExtension.Pages.BasePage

<h1>My options page</h1>
```

### Add a content script
Add the following to the `manifest.json`
```json
"content_scripts": [
  {
    "matches": [ "*://*/*" ],
    "js": [ "BrowserExtensionScripts/ContentScript.js" ]
  }
]
```
Add a `ContentScript.razor` Razor component under `Pages` folder with the following content.
```razor
@page "/contentscript"
@inherits Blazor.BrowserExtension.Pages.BasePage

<h1>My content script</h1>
```
Additional changes are required for content scripts to not have conflict of the element ID of the Blazor root component with any other elements in any pages.
1. In `index.html` change the ID `app` of line `<div id="app">Loading...</div>` to `%SafeProjectName%_app`.
2. In `Program.cs` change the ID `#app` of line `builder.RootComponents.Add<App>("#app");` to `#%SafeProjectName%_app`.

**Example:**

| Project Name | Safe Project Name | Element Name     |
| ------------ | ----------------- | ---------------- |
| MyProject    | MyProject         | MyProject_app    |
| My.Project   | My_Project        | My_Project_app   |
| My Project   | My_Project        | My_Project_app   |
| My-Project   | My_Project        | My_Project_app   |

In `App.razor`, add the following `if` statement to opt out of routing only for content scripts.
```razor
@using My.Project.Pages;
@inject NavigationManager Navigation

@if (Navigation.Uri.StartsWith("chrome-extension://"))
{
    <Router ...>
        ...
    </Router>
}
else
{
    <ContentScript></ContentScript>
}
```

## Browser Extension API
When you inherit from the `BasePage`, a few properties are injected for you to consume.
This includes the WebExtension API, Logger, JavaScript interop, etc.
The WebExtension API is provided by the package [WebExtension.Net](https://github.com/mingyaulee/WebExtension.Net), and you can consume the API in a page:
```razor
@inherits Blazor.BrowserExtension.Pages.BasePage;

<button @onclick="GetPluginIndexUrl">Get plugin Index page URL</button>
<p>@pluginIndexUrl</p>

@code {
    string pluginIndexUrl = null;
    async Task GetPluginIndexUrl()
    {
        pluginIndexUrl = await WebExtension.Runtime.GetURL("index.html");
    }
}
```

## How does routing work
You can use the `@page` directive to add route attribute to a Razor page, for example `@page "/options"`.
Usually in a server hosted application, you can access the route by just going to `domain.com/options`.
However in a browser extension, for example in Google Chrome, if you are try to access the route directly from the URL, e.g. `chrome-extension://extesion_id/options`, the background page will automatically intercept the request and redirect to `index.html?page=options`.  This is because the Blazor application is not hosted on a server, therefore only the static files are served in a browser extension.

## Customize build

The following MSBuild properties can be specified in your project file or when running `dotnet build` command.

| Property                      | Default value | Description                                                                    |
| ----------------------------- | ------------- | ------------------------------------------------------------------------------ |
| BrowserExtensionAssetsPath    | wwwroot       | The root folder where the JavaScript files should be added as link.            |
| BuildBlazorToBrowserExtension | true          | If set to false, the Blazor to Browser Extension build target will be skipped. |
| IncludeBrowserExtensionAssets | true          | If set to false, the JavaScript files will not be added as to the project.     |

## Additional Information
Find out how to build a cross browser extension with the links below:
1. [MDN - Building a cross-browser extension](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Build_a_cross_browser_extension)
2. [David Rousset - Creating One Browser Extension For All Browsers: Edge, Chrome, Firefox, Opera, Brave And Vivaldi](https://www.smashingmagazine.com/2017/04/browser-extension-edge-chrome-firefox-opera-brave-vivaldi/)