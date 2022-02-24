function Get-WindowsAutomaticUpdateLastResults {
    [OutputType([KPBSD.PowerShell.WindowsUpdate.AutomaticUpdateLastResults])]
    [CmdletBinding()]
    param(

    )
    process {
        $WUAUManager = Get-WindowsUpdateAutoUpdateManager
        [KPBSD.PowerShell.WindowsUpdate.AutomaticUpdateLastResults]::new(
            <# ServiceEnabled #> $WUAUManager.ServiceEnabled,
            <# Search UTC #> $WUAUManager.Results.LastSearchSuccessDate,
            <# Install UTC #> $WUAUManager.Results.LastInstallationSuccessDate
        )
    }
}