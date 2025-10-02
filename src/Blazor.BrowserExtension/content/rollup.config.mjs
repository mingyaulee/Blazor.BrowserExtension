import rollupCleanup from "./rollup-plugins/rollupCleanup.mjs";
import rollupNormalizeLineEndings from "./rollup-plugins/rollupNormalizeLineEndings.mjs";

export default [
  {
    input: ["src/BackgroundWorkerRunner.js", "src/ContentScript.js", "src/Blazor.BrowserExtension.lib.module.js"],
    output: {
      dir: "dist",
      chunkFileNames: "[name].js",
      format: "es" // ES module file
    },
    external: [
      "./lib/browser-polyfill.min.js",
      "../lib/decode.min.js",
      "../../JsBind.Net/JsBindNet.js"
    ],
    plugins: [
      rollupCleanup(),
      rollupNormalizeLineEndings()
    ]
  }
];