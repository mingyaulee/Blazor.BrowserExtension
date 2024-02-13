# Content script

Content scripts can be used to inject your Blazor application into any open browser tab.

Add the following to the `manifest.json`

**Manifest V3**

```json
"content_scripts": [
  {
    "matches": [ "*://*/*" ],
    "js": [ "content/Blazor.BrowserExtension/ContentScript.js" ]
  }
],
...
"web_accessible_resources": [
  {
    "resources": [
      ...
      "app.js"
    ],
    ...
  }
]
```

**Manifest V2**
```json
"content_scripts": [
  {
    "matches": [ "*://*/*" ],
    "js": [ "content/Blazor.BrowserExtension/ContentScript.js" ]
  }
],
...
"web_accessible_resources": [
  ...
  "app.js"
],
```

> **Tips**
>
> The `matches` property specifies the URL patterns when the content scripts should be injected into a browser tab.

Add a `ContentScript.razor` Razor component under `Pages` directory with the following content.

```razor
@page "/contentscript.html"
@inherits BasePage

<h1>My content script</h1>
```

> **Tips**
>
> Since the content scripts page is injected into browser tabs, you might want to have no layout or a different layout for this page.
> You can achieve this by using the `@layout` attribute.
>
> Read more about [Blazor layouts](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/layouts).


## Additional configurations for injecting Blazor in browser tabs

Additional changes are required for content scripts to not have conflict of the element ID of the Blazor root component with any other elements in any pages it is injected in.

First of all, decide on a unique ID to be used for the application `DIV` container, such as `My_Unique_Extension_App_Id` (**this ID is used in the steps below, replace with your own unique ID**).
It should be unique enough that it does not collide with any existing element in the pages where the content scripts are going to be injected.

1. In `index.html` change the line `<div id="app">` to `<div id="My_Unique_Extension_App_Id">`.
2. In `Program.cs` change the line `builder.RootComponents.Add<App>("#app");` to `builder.RootComponents.Add<App>("#My_Unique_Extension_App_Id");`.
3. If you don't have an [`app.js`](03_01_app.js.md) file yet, add a new file named `app.js` under the directory `wwwroot`.
4. Add the following code in the `app.js` file to inject a new `DIV` element with the matching ID into the current page.
   ```javascript
   /**
    * Called before Blazor starts.
    * @param {object} options Blazor WebAssembly start options. Refer to https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Platform/WebAssemblyStartOptions.ts
    * @param {object} extensions Extensions added during publishing
    * @param {object} blazorBrowserExtension Blazor browser extension instance
    */
   export function beforeStart(options, extensions, blazorBrowserExtension) {
     if (blazorBrowserExtension.BrowserExtension.Mode === blazorBrowserExtension.Modes.ContentScript) {
       const appDiv = document.createElement("div");
       appDiv.id = "My_Unique_Extension_App_Id";
       document.body.appendChild(appDiv);
     }
   }
   ```

In `App.razor`, add the following `if` statement to opt out of routing only for content scripts.

```razor
@using MyBlazorExtension.Pages;
@using Blazor.BrowserExtension;
@inject IBrowserExtensionEnvironment BrowserExtensionEnvironment

@if (BrowserExtensionEnvironment.Mode == BrowserExtensionMode.ContentScript)
{
    <ContentScript></ContentScript>
}
else
{
    <Router ...>
        ...
    </Router>
}
```

> **Tips**
>
> An alternative to this approach, allowing you to skip the additional configurations above, is to inject an iframe instead and set the `src` of the iframe to your extension's URL.
> ```javascript
> export function beforeStart(options, extensions, blazorBrowserExtension) {
>   if (blazorBrowserExtension.BrowserExtension.Mode === blazorBrowserExtension.Modes.ContentScript) {
>     const appDiv = document.createElement("div");
>     const appIframe = document.createElement("iframe");
>     appIframe.src = browser.runtime.getURL("contentscript.html");
>     appDiv.appendChild(appIframe);
>     // The div can be used to control the position and the size of the iframe in the page
>     document.body.appendChild(appDiv);
>   }
> }
> ```

## Using the Web Extensions API

Some APIs are not available in the content scripts and thus will require the content script to communicate with the background service worker/script in order to use these APIs.

Communication is possible using the `Runtime` API which allows one-time messaging or continuous messaging using a `Port` object.

# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/concepts/content-scripts)
- [Chrome for Developers - Messaging](https://developer.chrome.com/docs/extensions/develop/concepts/messaging)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Content_scripts)
