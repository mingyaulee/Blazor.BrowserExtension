const path = require("path");
const fs = require("fs");

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
        fileContent = fileContent.replace(/(\r\n|\n)/g, "\r\n");
        fs.writeFileSync(filePath, fileContent, { encoding: "utf8" });
      }
    }
  };
};

export default rollupNormalizeLineEndings;
