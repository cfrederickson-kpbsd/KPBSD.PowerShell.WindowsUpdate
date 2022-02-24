function Suspend-WindowsAutomaticUpdate {
    [CmdletBinding()]
    param()
    process {
        $WUAU = Get-WindowsUpdateAutoUpdateManager
        $WUAU.Pause()
    }
}