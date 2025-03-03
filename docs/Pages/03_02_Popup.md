# Popup page

A popup is shown when the user clicks on the extension action icon.

Add the following to the `manifest.json`

```json
"action": {
  "default_popup": "popup.html"
},
```

Add a `Popup.razor` Razor component under `Pages` directory with the following content.

```razor
@page "/popup.html"
@inherits BasePage

<h1>My popup page</h1>
```


# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/ui/add-popup)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/user_interface/Popups)
