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
    $baseUrl = "https://unpkg.com"
    $request = [System.Net.WebRequest]::Create("$baseUrl/webextension-polyfill")
    $request.AllowAutoRedirect = $false
    $response = $request.GetResponse()
    
    If ($response.StatusCode -eq "Found") {
        $versionRelativeUrl = $response.GetResponseHeader("Location")
        $versionUrl = "$baseUrl$versionRelativeUrl/dist"
        DownloadFile -baseUrl $versionUrl -fileName "browser-polyfill.js"
        DownloadFile -baseUrl $versionUrl -fileName "browser-polyfill.js.map"
        DownloadFile -baseUrl $versionUrl -fileName "browser-polyfill.min.js"
        DownloadFile -baseUrl $versionUrl -fileName "browser-polyfill.min.js.map"
    } Else {
        Throw "Unable to get the latest version of webextension-polyfill."
    }
}

Function UpdateBrotliDecode {
    $baseUrl = "https://raw.githubusercontent.com/google/brotli/master/js"
    DownloadFile -baseUrl $baseUrl -fileName "decode.js"
    DownloadFile -baseUrl $baseUrl -fileName "decode.min.js"
}

UpdateBrowserPolyfill
UpdateBrotliDecode