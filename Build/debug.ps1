param(
    [switch]
    $UseWindowsPowerShell
)

if ($UseWindowsPowerShell) {
    $Exe = 'powershell.exe'
}
else {
    $Exe = 'pwsh.exe'
}
while ($true) {
    & $Exe -noexit -noprofile -command "Import-Module '$PSScriptRoot/debug.psm1' -Verbose -ArgumentList `$true; "
}