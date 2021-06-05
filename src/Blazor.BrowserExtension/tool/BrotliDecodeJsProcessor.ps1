$jsPath = "../content/BrowserExtensionScripts/lib/decode.js"
$jsContent = Get-Content $jsPath -Raw
Function OctalToUnicode {
    param(
        $octal
    )
    Return ([Convert]::ToString([Convert]::ToInt32($octal, 8), 16)).PadLeft(4, "0")
}
$jsContent = [regex]::Replace($jsContent, "\\(\d{1,3})" ,{param($match) "\u$(OctalToUnicode($match.Groups[1].Value))"})
$jsContent | Set-Content $jsPath