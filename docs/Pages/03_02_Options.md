# Options page

An options page can be used to allow the user to change the extension settings and user preferences.

Add the following to the `manifest.json`

```json
"options_ui": {
  "page": "options.html",
  "open_in_tab": true
}
```

Add a `Options.razor` Razor component under `Pages` directory with the following content.

```razor
@page "/options.html"
@inherits BasePage

<h1>My options page</h1>
```


# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/ui/options-page)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/user_interface/Options_pages)
