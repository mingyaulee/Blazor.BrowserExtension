import path from "node:path";
import fs from "node:fs";

const rollupNormalizeLineEndings = function () {
  return {
    name: "normalizeLineEndings",
    writeBundle: function (options, bundle) {
      for (const chunk of Object.values(bundle)) {
        if (chunk.type !== "chunk") {
          continue;
        }

        const filePath = path.resolve(path.join(options.dir, chunk.fileName));
        let fileContent = fs.readFileSync(filePath, "utf8");
        fileContent = fileContent.replaceAll(/\r?\n/g, "\r\n");

        if (chunk.fileName == "ContentScript.js") {
          // Update import path to be absolute
          fileContent = fileContent.replaceAll("await import('./CoreInternal.js')", "await import(`${url}content/Blazor.BrowserExtension/CoreInternal.js`)");
        }

        fs.writeFileSync(filePath, fileContent, { encoding: "utf8" });
      }
    }
  };
};

export default rollupNormalizeLineEndings;
