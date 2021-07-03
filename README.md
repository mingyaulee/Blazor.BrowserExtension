# Blazor.BrowserExtension
[![Nuget](https://img.shields.io/nuget/v/Blazor.BrowserExtension?style=flat-square&color=blue)](https://www.nuget.org/packages/Blazor.BrowserExtension/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mingyaulee/Blazor.BrowserExtension/Build?style=flat-square&color=blue)](https://github.com/mingyaulee/Blazor.BrowserExtension/actions/workflows/Blazor.BrowserExtension-Build.yml)
[![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/Blazor.BrowserExtension?server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/dashboard?id=Blazor.BrowserExtension)

You can now easily build a browser extension with Blazor!

## Demo
![Blazor Browser Extension Demo](Demo.gif)

## How to use this package

> **Important for v0.\*.\*:**<br />
> This package is still in pre-release stage so the versioning does not comply with semantic versioning. Feature and bug fix increments the patch version and breaking change increments the minor version. So be sure to check the release note before upgrading between minor version.

This package imports two other packages, which are:
1. [WebExtensions.Net](https://github.com/mingyaulee/WebExtensions.Net) - Provides interop for WebExtensions standard API.
2. Blazor.BrowserExtension.Build (in this repository) - Adds build target and tasks to the project.

### Create new project
1. Run `dotnet new --install Blazor.BrowserExtension.Template`.
0. Run `dotnet new browserext --name <ProjectName>` to initialize a new project with the template.
0. Change the working directory into the newly created project directory.
0. Run `dotnet build` to build the project.

> If you are using Visual Studio, you can do all these from the UI, like how I did in the demo above (once you have enabled showing .Net Core templates in the New project dialog).

### Setup existing project
1. Install NuGet package `Blazor.BrowserExtension`.
0. Add `<BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>` under the `<PropertyGroup>` node in your `.csproj` project file to automatically setup the project files to be compatible for building into browser extension.
0. Build the project.

### Manual Setting Up
You can setup the project manually as well, if for some reason you encounter any problem with the bootstrapping step above.
1. Add a new file `manifest.json` under the `wwwroot` folder. An example of minimal `manifest.json` file:
   ```json
   {
     "manifest_version": 2,
     "name": "My Blazor Extension",
     "description": "My browser extension built with Blazor WebAssembly",
     "version": "0.1",
     "background": {
       "page": "background.html",
       "persistent": true
     },
     "content_security_policy": "script-src 'self' 'unsafe-eval' 'wasm-eval' 'sha256-v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA='; object-src 'self'",
     "web_accessible_resources": [
       "framework/*",
       "BrowserExtensionScripts/*",
       "WebExtensionsScripts/*"
     ]
   }
   ```
0. Add the following to the `.csproj` file to make sure that all the files under `wwwroot` will always be copied to the output.
   ```xml
     <ItemGroup>
       <None Include="wwwroot\**\*" CopyToOutputDirectory="Always" />
     </ItemGroup>
   ```
0. In `wwwroot/index.html` replace the script tag `<script src="_framework/blazor.webassembly.js"></script>` with `<script src="BrowserExtensionScripts/Core.js"></script>`
0. In `Pages/Index.razor` replace the first line `@page "/"` with the following lines:
   ```razor
   @page "/index.html"
   @inherits Blazor.BrowserExtension.Pages.IndexPage
   ```
0. Add a `Background.razor` file under `Pages` folder (Right click on the `Pages` folder and select Add → Razor Component), with the following content:
   ```razor
   @page "/background.html"
   @inherits Blazor.BrowserExtension.Pages.BackgroundPage
   @using WebExtensions.Net.Tabs
   
   @code {
       protected override async Task OnInitializedAsync()
       {
           await base.OnInitializedAsync();
           // this opens index.html in the extension as a new tab when the background page is loaded
           var extensionUrl = await WebExtensions.Runtime.GetURL("index.html");
           await WebExtensions.Tabs.Create(new CreateProperties()
           {
               url = extensionUrl
           });
       }
   }
   ```
0. Add the following into `Program.cs` file.
   ```csharp
   public static async Task Main(string[] args)
   {
       ...
       // workaround to use JavaScript fetch to bypass url validation
       // see: https://github.com/dotnet/runtime/issues/52836
       builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
       builder.Services.AddBrowserExtensionServices(options =>
       {
           options.ProjectNamespace = typeof(Program).Namespace;
       });
       ...
   }
   ```

## Change initialization behaviour
The following properties can be set to change the behaviour of the core scripts.

| Variable Name               | Description                                                                                                                                        |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| ImportBrowserPolyfill       | Set to `false` to disable importing of the browser polyfill script.<br />Default: `true`                                                           |
| StartBlazorBrowserExtension | Set to `false` to prevent auto initialization of Blazor. Use `BlazorBrowserExtension.InitializeAsync` to initialize manually.<br />Default: `true` |

**Example:**

In `wwwroot/index.html`

```html
<script>
  globalThis.ImportBrowserPolyfill = false;
  globalThis.StartBlazorBrowserExtension = false;
</script>
<script src="BrowserExtensionScripts/Core.js"></script>
<script>
  globalThis.BlazorBrowserExtension.InitializeAsync("Production");
</script>
```

## Build and load extension
### Google Chrome
1. Launch the Extensions page ( ⋮ → More tools → Extensions).
2. Switch on `Developer mode`.
3. Click on the `Load unpacked` button, then navigate to `%ProjectDir%\bin\Debug\net5.0\` and select the foler `wwwroot`.

### Microsoft Edge
1. Launch the Extensions page ( ⋮ → Extensions).
2. Click on the ☰ and switch on `Developer mode`.
3. Click on the button with the title `Load unpacked`, then navigate to `%ProjectDir%\bin\Debug\net5.0\` and select the foler `wwwroot`.

### Mozilla Firefox
1. Navigate to the URL [about:debugging#/runtime/this-firefox](about:debugging#/runtime/this-firefox)
2. Click on `Load Temporary Add-on...`, then navigate to `%ProjectDir%\bin\Debug\net5.0\wwwroot` and select any file in the directory.

## Debugging locally in IIS Express or Kestrel
1. Start the Blazor project directly from Visual Studio or `dotnet run`.
0. Once the application is loaded, use the Blazor debugging hotkey Shift+Alt+D to launch the debugging console.

## Browser Extension features

### Add a browser action popup page
Add the following to the `manifest.json`
```json
"browser_action": {
  "default_popup": "popup.html"
}
```
Add a `Popup.razor` Razor component under `Pages` folder with the following content.
```razor
@page "/popup.html"
@inherits Blazor.BrowserExtension.Pages.BasePage

<h1>My popup page</h1>
```

### Add an extension options page
Add the following to the `manifest.json`
```json
"options_ui": {
  "page": "options.html",
  "open_in_tab": true
}
```
Add a `Options.razor` Razor component under `Pages` folder with the following content.
```razor
@page "/options.html"
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
@page "/contentscript.html"
@inherits Blazor.BrowserExtension.Pages.BasePage

<h1>My content script</h1>
```
Additional changes are required for content scripts to not have conflict of the element ID of the Blazor root component with any other elements in any pages.
1. In `index.html` change the ID `app` of line `<div id="app">Loading...</div>` to `%SafeProjectName%_app`.
2. In `Program.cs` change the ID `#app` of line `builder.RootComponents.Add<App>("#app");` to `#%SafeProjectName%_app`.

**Example:**

| Project Name | Safe Project Name | Element Name   |
| ------------ | ----------------- | -------------- |
| MyProject    | MyProject         | MyProject_app  |
| My.Project   | My_Project        | My_Project_app |
| My Project   | My_Project        | My_Project_app |
| My-Project   | My_Project        | My_Project_app |

In `App.razor`, add the following `if` statement to opt out of routing only for content scripts.
```razor
@using My.Project.Pages;
@inject IBrowserExtensionEnvironment BrowserExtensionEnvironment

@if (BrowserExtensionEnvironment.Mode == BrowserExtensionMode.ContentScript)
{
    <ContentScript></ContentScript>
}
else
{
    <Router ...>
        ...
    </Router>
}
```

## Browser Extension API
When you inherit from the `BasePage`, a few properties are injected for you to consume.
This includes the WebExtensions API and Logger.
The WebExtensions API is provided by the package [WebExtensions.Net](https://github.com/mingyaulee/WebExtensions.Net), and you can consume the API in a page:
```razor
@inherits Blazor.BrowserExtension.Pages.BasePage;

<button @onclick="GetIndexPageUrl">Get index page URL</button>
<p>@indexPageUrl</p>

@code {
    string indexPageUrl = null;
    async Task GetIndexPageUrl()
    {
        indexPageUrl = await WebExtensions.Runtime.GetURL("index.html");
    }
}
```

## How does routing work
### Default routing
The default routing when using this package is physical file routing.
When building the project,
1. all the razor components are processed to get a list of all the physical file routes
0. the routing entry file (default is `index.html`, see `BrowserExtensionRoutingEntryFile` below) is copied to the output directory based on the list of physical file routes

For example, if the `Background.razor` contains `@page "/background.html"` and the `Options.razor` contains `@page "/options.html"`, when the project is built or published, the file `index.html` will be copied/duplicated to the output directory with the name `background.html` and `options.html`.

This is especially useful for browser extensions because the browsers only serve static files, and the presence of these physical files supports the routing when the extension page is reloaded.

### Virtual path routing
Routing with virtual path is also supported, however not encouraged due to the requirements.

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
> The background will only intercept the calls if it detects the permissions are declared.
> When the permissions are not declared in the manifest, the only routing that works is the physical file routing.

## Customize build

The following MSBuild properties can be specified in your project file or when running `dotnet run`, `dotnet build` and `dotnet publish` command.

| Property                          | Default value                                        | Description                                                                      |
| --------------------------------- | ---------------------------------------------------- | -------------------------------------------------------------------------------- |
| BrowserExtensionEnvironment       | Blazor default: Production                           | The environment name which the Blazor application will run in.                   |
| IncludeBrowserExtensionAssets     | True                                                 | If set to False, the JavaScript files will not be added as to the project.       |
| BrowserExtensionBootstrap         | False                                                | If set to True, the project will be bootstrapped during the build.               |
| BuildBlazorToBrowserExtension     | True                                                 | If set to False, the Blazor to Browser Extension build target will be skipped.   |
| PublishBlazorToBrowserExtension   | True                                                 | If set to False, the Blazor to Browser Extension publish target will be skipped. |
| BrowserExtensionAssetsPath        | wwwroot                                              | The root folder where the JavaScript files should be added as link.              |
| BrowserExtensionRoutingEntryFile  | index.html                                           | The HTML entry file for the Blazor application.                                  |
| BrowserExtensionEnableCompression | $(BlazorEnableCompression)<br />Blazor default: True | If set to True, the .br compressed files will be loaded instead of .dll.         |

## Additional Information
Find out how to build a cross browser extension with the links below:
1. [MDN - Building a cross-browser extension](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Build_a_cross_browser_extension)
2. [David Rousset - Creating One Browser Extension For All Browsers: Edge, Chrome, Firefox, Opera, Brave And Vivaldi](https://www.smashingmagazine.com/2017/04/browser-extension-edge-chrome-firefox-opera-brave-vivaldi/)
