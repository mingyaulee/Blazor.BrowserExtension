const darkModeStorageKey = "DarkMode";
let currentMode = "";
const toggleDarkMode = () => {
  if (currentMode) {
    currentMode = "";
    globalThis.document.documentElement.dataset.bsTheme = "";
    globalThis.localStorage.setItem(darkModeStorageKey, "disabled");
  } else {
    currentMode = "dark";
    globalThis.document.documentElement.dataset.bsTheme = "dark";
    globalThis.localStorage.setItem(darkModeStorageKey, "enabled");
  }
};

const initializeMode = () => {
  let darkMode = globalThis.localStorage.getItem(darkModeStorageKey);
  if (!darkMode) {
    darkMode = globalThis.matchMedia?.('(prefers-color-scheme: dark)').matches;
  }

  if (darkMode && darkMode != "disabled") {
    toggleDarkMode();
  }
};

const onPageRender = () => {
  document.getElementsByTagName("article")[0]
    .querySelectorAll("div.code-toolbar")
    .forEach(toolbar => toolbar.childElementCount === 1 && toolbar.remove());
  globalThis.Prism.highlightAll();
};

globalThis.appUtils = {
  toggleDarkMode,
  initializeMode,
  onPageRender
};

let brotliImport;
const getBrotliDecode = async () => {
  if (!brotliImport) {
    brotliImport = import("../lib/decode.min.js");
  }
  return (await brotliImport).BrotliDecode;
};

Blazor.start({
  loadBootResource: function (type, name, defaultUri, integrity) {
    if (type !== 'dotnetjs' && location.hostname !== 'localhost' && type !== 'configuration') {
      return (async function () {
        const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
        if (!response.ok) {
          throw new Error(response.statusText);
        }
        const originalResponseBuffer = await response.arrayBuffer();
        const originalResponseArray = new Int8Array(originalResponseBuffer);
        const decompressedResponseArray = (await getBrotliDecode())(originalResponseArray);
        const contentType = type ===
          'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
        return new Response(decompressedResponseArray,
          { headers: { 'content-type': contentType } });
      })();
    }
  }
});
