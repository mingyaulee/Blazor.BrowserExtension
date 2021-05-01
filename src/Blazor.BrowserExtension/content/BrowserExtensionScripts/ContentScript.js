(async () => {
  const projectName = "__ProjectName__";

  const appDiv = document.createElement("div");
  appDiv.id = `${projectName}_app`;
  document.body.appendChild(appDiv);
  // @ts-ignore JS is not a module
  await import("./Core.js");
})();