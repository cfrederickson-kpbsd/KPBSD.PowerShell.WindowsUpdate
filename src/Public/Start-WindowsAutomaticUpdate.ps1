function Start-WindowsAutomaticUpdate {
    [CmdletBinding()]
    param()
    process {
        $WUAU = Get-WindowsUpdateAutoUpdateManager
        $WUAU.DetectNow()
    }
}