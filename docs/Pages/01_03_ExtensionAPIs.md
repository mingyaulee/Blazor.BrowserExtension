# APIs for Browser Extension

There are [differences](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Browser_support_for_JavaScript_APIs) in some APIs available in different browsers, most of the general APIs are standardised with the use of [Mozilla's WebExtension polyfill](https://github.com/mozilla/webextension-polyfill).

The APIs are available in JavaScript and most of them have been ported to Blazor from the package [WebExtensions.Net](https://github.com/mingyaulee/WebExtensions.Net), which uses `IJSRuntime` to interop with the standard extension APIs.

When you create a new Razor page, add `@inherits BasePage` on top to get access to the property exposed by the `WebExtensions` property.

```razor
@inherits BasePage;

<button @onclick="GetOptionsPageUrl">Get options page URL</button>
<p>@optionsPageUrl</p>

@code {
    string optionsPageUrl = null;
    async Task GetOptionsPageUrl()
    {
        optionsPageUrl = await WebExtensions.Runtime.GetURL("options.html");
    }
}
```

Each of the property in the `IWebExtensionsApi` class provides a set of APIs for different functionalities that can be implemented by an extension. These property sometimes include description of additional setup required to use the API.

For example, the `Alarms` API shows that it requires manifest permission `alarms`.

```csharp
public interface IWebExtensionsApi
{
    //
    // Summary:
    //     Requires manifest permission alarms.
    IAlarmsApi Alarms { get; }
}
```

# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/reference/api)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API)
