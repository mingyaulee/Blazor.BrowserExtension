# Debugging the extension

1. Start the Blazor project directly from Visual Studio, or use the command `dotnet run`, or the command `dotnet watch` for hot reload.
0. Load the extension in the browser as described in the next section.
0. Add a breakpoint from Visual Studio/Visual Studio Code, or use the Blazor debugging hotkey `Shift+Alt+D` to launch the debugging console.

# Loading the extension

**Google Chrome**

1. Launch the Extensions page ( ⋮ → More tools → Extensions).
2. Switch on `Developer mode`.
3. Click on the `Load unpacked` button, then navigate to `bin/Debug/net[NetVersion].0` and select the `browserextension` directory.

**Microsoft Edge**

1. Launch the Extensions page ( ⋮ → Extensions).
2. Click on the ☰ and switch on `Developer mode`.
3. Click on the button with the title `Load unpacked`, then navigate to `bin/Debug/net[NetVersion].0` and select the `browserextension` directory.

**Mozilla Firefox**

1. Navigate to the URL [about:debugging#/runtime/this-firefox](about:debugging#/runtime/this-firefox). You need to copy this link and paste it in the address bar.
2. Click on `Load Temporary Add-on...`, then navigate to `bin/Debug/net[NetVersion].0/browserextension` and select any file in the directory.
   > **Tips**
   >
   > For Mozilla Firefox, the extension manifest needs to be modified before you can load it.
   > See [Background Worker](03_02_BackgroundWorker.md#mozilla-firefox) page.

# Debugging without loading the extension in the browser

When running outside of the browser extension context, all the browser extension APIs are not available.
The behaviour can be mocked by configuring the `MockResolvers`.

```csharp
MockResolvers.Configure(configure =>
{
    // Configure the behaviour of the mock APIs you are expecting to use when debugging
    configure.Api.Property(api => api.Runtime.Id).ReturnsForAnyArgs("myextensionid");
});
```

The [WebExtensions.Net page](https://github.com/mingyaulee/WebExtensions.Net) shows more examples of configuring the `MockResolvers`.
Some APIs have been mocked by default in [WebExtensions.Net `DefaultMockResolver`](https://github.com/mingyaulee/WebExtensions.Net/blob/main/src/WebExtensions.Net/Mock/Resolvers/DefaultMockResolver.cs#L55).

# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/get-started/tutorial/hello-world#load-unpacked)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Your_first_WebExtension#installing)
