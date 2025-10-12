$relativeLibPath = "../src/Blazor.BrowserExtension/content/src/lib"
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$libPath = Resolve-Path "$scriptPath/$relativeLibPath"

If (-Not(Test-Path -Path $libPath)) {
    Throw "Library path does not exist: $libPath"
}

Function DownloadFile {
    param (
        $baseUrl,
        $fileName
    )
    Invoke-RestMethod -Uri "$baseUrl/$fileName" -OutFile "$libPath/$fileName"
}

Function UpdateBrowserPolyfill {
    $baseUrl = "https://unpkg.com/webextension-polyfill/dist"
    DownloadFile -baseUrl $baseUrl -fileName "browser-polyfill.js"
    DownloadFile -baseUrl $baseUrl -fileName "browser-polyfill.js.map"
    DownloadFile -baseUrl $baseUrl -fileName "browser-polyfill.min.js"
    DownloadFile -baseUrl $baseUrl -fileName "browser-polyfill.min.js.map"
}

Function UpdateBrotliDecode {
    $baseUrl = "https://raw.githubusercontent.com/google/brotli/master/js"
    DownloadFile -baseUrl $baseUrl -fileName "decode.js"
    DownloadFile -baseUrl $baseUrl -fileName "decode.min.js"
}

UpdateBrowserPolyfill
UpdateBrotliDecode