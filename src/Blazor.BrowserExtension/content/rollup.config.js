import rollupCleanup from "./rollup-plugins/rollupCleanup.js";
import rollupNormalizeLineEndings from "./rollup-plugins/rollupNormalizeLineEndings.js";

export default [
  {
    input: [ "src/ContentScript.js", "src/Core.js" ],
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