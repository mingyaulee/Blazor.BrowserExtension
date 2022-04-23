# Setup existing project
1. Install NuGet package `Blazor.BrowserExtension`.
0. Add `<BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>` under the `<PropertyGroup>` node in your `.csproj` project file to automatically setup the project files to be compatible for building into browser extension.
0. Build the project.

## Manual Setting Up
You can setup the project manually as well, if for some reason you encounter any problem with the bootstrapping step above.
1. Add a new file `manifest.json` under the `wwwroot` folder. An example of minimal `manifest.json` file:
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
0. In `wwwroot/index.html` replace the script tag `<script src="_framework/blazor.webassembly.js"></script>` with `<script src="content/Blazor.BrowserExtension/Core.js"></script>`
0. In `Pages/Index.razor` replace the first line `@page "/"` with the following lines:
   ```razor
   @page "/index.html"
   @inherits Blazor.BrowserExtension.Pages.IndexPage
   ```
0. Add a `BackgroundWorker.js` file under `wwwroot` folder, with the following content:
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
   If you are targeting .Net 5.0 you will need to add replace the original `HttpClient` service registration with
   ```csharp
   public static async Task Main(string[] args)
   {
       ...
       // workaround to use JavaScript fetch to bypass url validation
       // see: https://github.com/dotnet/runtime/issues/52836 (fixed in .Net 6)
       builder.Services.AddScoped<HttpClient>(sp => new JsHttpClient(sp) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
       ...
   }
   ```
