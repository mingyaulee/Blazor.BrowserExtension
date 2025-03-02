# Publishing the extension

1. Run the `dotnet publish` command to publish the extension.
0. The extension output is in the `bin/Release/net9.0/publish/browserextension` directory.
0. The contents of this directory should be used when packing your extension. The contents can also be loaded unpacked the same way as the build output.

## Packing the extension

Typically the extension stores require that you pack your extension when submitting it to the store.
Most of them require just a simple zip format, where the zipped file is renamed with `.crx` extension.

Refer to the guidance for the respective store for more details.

- [Chrome Web Store](https://developer.chrome.com/docs/webstore/publish)
- [Microsoft Edge Add-ons](https://learn.microsoft.com/en-us/microsoft-edge/extensions-chromium/publish/publish-extension)
- [AMO (addons.mozilla.org)](https://extensionworkshop.com/documentation/publish/submitting-an-add-on/)
- [Opera addons](https://dev.opera.com/extensions/publishing-guidelines/)
- [Safari App Store](https://developer.apple.com/documentation/safariservices/safari_web_extensions/distributing_your_safari_web_extension)

## Optimization

Built-in optimization has been done by this package to use Brotli compression by default, removing all the uncompressed files from the publish output to reduce the package size.

If you would like to use AOT compilation, it is best to benchmark the size before and after using AOT compilation as your extension might end up having an increase in size.

You can also enable IL trimming when publishing and disable unused features such as timezone support.

Refer to [Blazor performance best practices](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance?view=aspnetcore-8.0#minimize-app-download-size) documentation page for more information.
