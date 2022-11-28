const rollupCleanup = function () {
  const singleLineRegex = /\/\*\* *([^\*])*\*\/(\n|\r\n)?/.toString();
  const multiLineRegex = / *\/\*\* *([^\*]|\*[^\/])*\*\/(\n|\r\n)+/.toString();
  const regex = `(${singleLineRegex.substring(1, singleLineRegex.length - 1)})|(${multiLineRegex.substring(1, multiLineRegex.length - 1)})`;
  return {
    name: "cleanup",
    transform: function (/** @type {string} */code) {
      return code.replace(new RegExp(regex, "g"), "");
    }
  };
};

export default rollupCleanup;
