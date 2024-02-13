# Setup existing project

1. Install the latest `Blazor.BrowserExtension` NuGet package to the Blazor project.
0. Add `<BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>` under the `<PropertyGroup>` node in your `.csproj` project file.
0. Build the project.
0. This should automatically setup the project files to be compatible for building into browser extension and the `BrowserExtensionBootstrap` property is removed from the `.csproj` project file.

## Manual Setting Up

You can setup the project manually as well, if for some reason you encounter any problem with the bootstrapping step above.

1. Add a new file `manifest.json` under the `wwwroot` directory. An example of minimal `manifest.json` file:
   ```json
   {
     "manifest_version": 3,
     "name": "My Blazor Extension",
     "description": "My browser extension built with Blazor WebAssembly",
     "version": "0.1",
     "background": {
       "service_worker": "BackgroundWorker.js",
       "type": "module"
     },
     "content_security_policy": {
       "extension_pages": "script-src 'self' 'wasm-unsafe-eval'; object-src 'self'"
     },
     "web_accessible_resources": [
       {
         "resources": [
           "framework/*",
           "content/*"
         ],
         "matches": [ "<all_urls>" ]
       }
     ]
   }
   ```
0. Add the following to the `.csproj` file to make sure that all the files under `wwwroot` will always be copied to the output.
   ```xml
     <ItemGroup>
       <None Include="wwwroot\**\*" CopyToOutputDirectory="Always" />
     </ItemGroup>
   ```
0. In `wwwroot/index.html` remove the `_` in the `_framework` script tag, which will then look like
   ```html
   <script src="framework/blazor.webassembly.js"></script>
   ```
0. Add `@using Blazor.BrowserExtension.Pages` into `_Imports.razor` file.
0. In `Pages/Index.razor` replace the first line `@page "/"` with the following lines:
   ```razor
   @page "/index.html"
   @inherits IndexPage
   ```
0. Add a `BackgroundWorker.js` file under `wwwroot` directory, with the following content:
   ```js
   // Import for the side effect of defining a global 'browser' variable
   import * as _ from "/content/Blazor.BrowserExtension/lib/browser-polyfill.min.js";
   
   browser.runtime.onInstalled.addListener(() => {
     const indexPageUrl = browser.runtime.getURL("index.html");
     browser.tabs.create({
       url: indexPageUrl
     });
   });
   ```
0. Add the following into `Program.cs` file.
   ```csharp
   public static async Task Main(string[] args)
   {
       ...
       builder.Services.AddBrowserExtensionServices();
       ...
   }
   ```
