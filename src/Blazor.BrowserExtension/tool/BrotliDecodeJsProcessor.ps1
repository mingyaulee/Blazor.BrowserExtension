param (
    $jsPath
)

$jsContent = Get-Content $jsPath -Raw
Function OctalToUnicode {
    param(
        $octal
    )
    Return ([Convert]::ToString([Convert]::ToInt32($octal, 8), 16)).PadLeft(4, "0")
}
$jsContentHeader = "// This file has been transformed by tool/BrotliDecodeJsProcessor.ps1 to convert octal character literals to unicode character literals`r`n"
$jsContent = $jsContentHeader + [regex]::Replace($jsContent, "\\(\d{1,3})" ,{param($match) "\u$(OctalToUnicode($match.Groups[1].Value))"})
$jsContent | Set-Content $jsPath -NoNewline