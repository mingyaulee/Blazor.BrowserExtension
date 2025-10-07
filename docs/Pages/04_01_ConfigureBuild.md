# Configure build and publish

The following MSBuild properties can be specified in your `.csproj` project file or when running `dotnet build`, `dotnet publish`, `dotnet run` or `dotnet watch` command.

| Property                                 | Default value                                                                                                    | Description                                                                                        |
| ---------------------------------------- | ---------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| BrowserExtensionEnvironment              | Blazor default: `Production`                                                                                     | The environment name which the Blazor application will run in.                                     |
| IncludeBrowserExtensionAssets            | `true`                                                                                                           | If set to `false`, the JavaScript files from this package will not be added to the project.        |
| BrowserExtensionBootstrap                | `false`                                                                                                          | If set to `true`, the project will be bootstrapped during the build.                               |
| BuildBlazorToBrowserExtension            | `true`                                                                                                           | If set to `false`, the Blazor to Browser Extension build target will be skipped.                   |
| PublishBlazorToBrowserExtension          | `true`                                                                                                           | If set to `false`, the Blazor to Browser Extension publish target will be skipped.                 |
| GenerateBrowserExtensionBackgroundWorker | `true`                                                                                                           | If set to `false`, the generation of `content/BackgroundWorker.js` will be skipped.                |
| BrowserExtensionAssetsPath               | `wwwroot`                                                                                                        | The root directory of the browser extension assets.                                                |
| BrowserExtensionOutputPath               | `browserextension`                                                                                               | The directory of the build/publish output.                                                         |
| BrowserExtensionRoutingEntryFile         | `index.html`                                                                                                     | The HTML entry file for the Blazor application.                                                    |
| BrowserExtensionEnableCompression        | `$(CompressionEnabled)`<br />Blazor default: `true` | **When published**<br />If set to `true`, the .br compressed files will be loaded instead of .dll. |


**Add property to `.csproj` project file**

```xml
<PropertyGroup>
  <TargetFramework>net[NetVersion].0</TargetFramework>
  <BrowserExtensionEnvironment>Development</BrowserExtensionEnvironment>
</PropertyGroup>
```


**Add property to `dotnet build`, `dotnet publish` or `dotnet run`**

```shell
dotnet build -p:BrowserExtensionEnvironment=Development
```

or

```shell
dotnet build --property:BrowserExtensionEnvironment=Development
```


**Add property to `dotnet watch`**

The `-p` abbreviation is not supported for `dotnet watch` because it was being used as the abbreviation for `--project` but it is deprecated now.

```shell
dotnet watch --property:BrowserExtensionEnvironment=Development
```
