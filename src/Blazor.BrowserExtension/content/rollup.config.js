export default [
  {
    input: ['src/ContentScript.js', 'src/Core.js'],
    output: {
      dir: 'dist',
      format: 'es' // ES module file
    },
    external: [
      "./lib/browser-polyfill.min.js",
      "../lib/decode.min.js",
      "../../JsBind.Net/JsBindNet.js"
    ]
  }
];