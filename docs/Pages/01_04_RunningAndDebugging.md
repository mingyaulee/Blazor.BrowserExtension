# Running the extension in the browser

**Google Chrome**

1. Launch the Extensions page ( ⋮ → More tools → Extensions).
2. Switch on `Developer mode`.
3. Click on the `Load unpacked` button, then navigate to `bin/Debug/net8.0` and select the `browserextension` directory.

**Microsoft Edge**

1. Launch the Extensions page ( ⋮ → Extensions).
2. Click on the ☰ and switch on `Developer mode`.
3. Click on the button with the title `Load unpacked`, then navigate to `bin/Debug/net8.0` and select the `browserextension` directory.

**Mozilla Firefox**

1. Navigate to the URL [about:debugging#/runtime/this-firefox](about:debugging#/runtime/this-firefox). You need to copy this link and paste it in the address bar.
2. Click on `Load Temporary Add-on...`, then navigate to `bin/Debug/net8.0/browserextension` and select any file in the directory.
   > **Tips**
   >
   > For Mozilla Firefox, the extension manifest needs to be modified before you can load it.
   > See [Background Worker](03_02_BackgroundWorker.md#mozilla-firefox) page.

# Debugging

1. Start the Blazor project directly from Visual Studio, or use the command `dotnet run`, or the command `dotnet watch` for hot reload.
0. Once the application is loaded, use the Blazor debugging hotkey `Shift+Alt+D` to launch the debugging console.

This debugging method is running outside of the browser extension context, thus all the APIs are not available when debugging and can be mocked by configuring the `MockResolvers`.

```csharp
MockResolvers.Configure(configure =>
{
    // Configure the behaviour of the mock APIs you are expecting to use when debugging
    configure.Api.Property(api => api.Runtime.Id).ReturnsForAnyArgs("myextensionid");
});
```

The [WebExtensions.Net page](https://github.com/mingyaulee/WebExtensions.Net) shows more examples of configuring the `MockResolvers`.
Some APIs have been mocked by default in [WebExtensions.Net `DefaultMockResolver`](https://github.com/mingyaulee/WebExtensions.Net/blob/3eef452e6e36f3320c7c5a14dc6862223b714a06/src/WebExtensions.Net/Mock/Resolvers/DefaultMockResolver.cs#L55).

> **Important Note**
>
> At the moment, debugging when the application is loaded as an extension in the browser is not possible.
>
> This is because debugging requires a NodeJs debugging proxy launched by the DevServer, which is not available when loaded as an extension in the browser.
>
> The best way to debug when loaded as an extension is to use `Console.WriteLine` which writes the logs to the browser console window, accessible from the Developer Tools window (`F12`).

# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/get-started/tutorial/hello-world#load-unpacked)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Your_first_WebExtension#installing)
