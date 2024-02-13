(function () {
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
})();