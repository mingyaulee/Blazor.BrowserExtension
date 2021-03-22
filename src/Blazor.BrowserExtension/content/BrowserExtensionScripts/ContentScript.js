(async () => {
    const appDiv = document.createElement("div");
    appDiv.id = "__ProjectName___app";
    document.body.appendChild(appDiv);
    await import("./Core.js");
})();