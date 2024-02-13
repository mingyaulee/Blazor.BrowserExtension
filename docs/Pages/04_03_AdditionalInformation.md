# Additional Information

Find out how to build a cross browser extension with the links below:

1. [MDN - Building a cross-browser extension](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Build_a_cross_browser_extension)
2. [David Rousset - Creating One Browser Extension For All Browsers: Edge, Chrome, Firefox, Opera, Brave And Vivaldi](https://www.smashingmagazine.com/2017/04/browser-extension-edge-chrome-firefox-opera-brave-vivaldi/)

If you want to publish the extension to the browser extension stores, do take note of the following (courtesy of [@dragnilar](https://github.com/mingyaulee/Blazor.BrowserExtension/issues/39)):

- Publishing to the Microsoft Edge Add-ons store is easier as long as you note somewhere in the testing steps and/or description that the extension uses Blazor. It's slower than Google but it's pretty much a hassle free experience.
- Publishing to Google Chrome Extension store is more difficult. They will reject your extension at first if you declare any chromium apis in the manifest. Their automated testing doesn't work with Blazor / WASM yet but the support team will help you to resolve the challenges throughout the process.
