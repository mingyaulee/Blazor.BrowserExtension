# Setup existing project

1. Install the latest `Blazor.BrowserExtension` NuGet package to the Blazor project.
0. Add `<BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>` under the `<PropertyGroup>` node in your `.csproj` project file.
0. Build the project.
0. This should automatically set up the project files to be compatible for building into browser extension and the `BrowserExtensionBootstrap` property is removed from the `.csproj` project file.
0. Ideally you should be able to repeat these steps countless times. However, if it fails, you can manually set up parts of the project by referring to the steps below.

## Manual Setting Up

You can set up the project manually as well, if for some reason you encounter any problem with the bootstrapping step above.

1. Add a new file `manifest.json` under the `wwwroot` directory. An example of minimal `manifest.json` file:
   ```json
   {
     "manifest_version": 3,
     "name": "My Blazor Extension",
     "description": "My browser extension built with Blazor WebAssembly",
     "version": "0.1",
     "background": {
       "service_worker": "content/BackgroundWorker.js",
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
0. Add `@using Blazor.BrowserExtension.Pages` into the `_Imports.razor` file.
0. In `Pages/Index.razor` replace the first line `@page "/"` with the following lines:
   ```razor
   @page "/index.html"
   @inherits IndexPage
   ```
0. Add a `BackgroundWorker.cs` class, with the following content.
   Add `using Blazor.BrowserExtension;` statement if needed.
   ```csharp
   public partial class BackgroundWorker : BackgroundWorkerBase
   {
       [BackgroundWorkerMain]
       public override void Main()
       {
           WebExtensions.Runtime.OnInstalled.AddListener(OnInstalled);
       }

       async Task OnInstalled()
       {
           var indexPageUrl = await WebExtensions.Runtime.GetURL("index.html");
           await WebExtensions.Tabs.Create(new()
           {
               Url = indexPageUrl
           });
       }
   }
   ```
0. Add the following into the `Program.cs` file to wrap the `RootComponents` setup.
   Add `using Blazor.BrowserExtension;` statement if needed.
   ```csharp
   public static async Task Main(string[] args)
   {
       ...
       builder.UseBrowserExtension(browserExtension =>
       {
           if (browserExtension.Mode == BrowserExtensionMode.Background)
           {
               builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();
           }
           else
           {
               builder.RootComponents.Add<App>("#app");
               builder.RootComponents.Add<HeadOutlet>("head::after");
           }
       });
       ...
   }
   ```
