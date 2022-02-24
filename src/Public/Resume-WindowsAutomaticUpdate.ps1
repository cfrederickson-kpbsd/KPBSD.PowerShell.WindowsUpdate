function Resume-WindowsAutomaticUpdate {
    [CmdletBinding()]
    param()
    process {
        $WUAU = Get-WindowsUpdateAutoUpdateManager
        $WUAU.Resume()
    }
}