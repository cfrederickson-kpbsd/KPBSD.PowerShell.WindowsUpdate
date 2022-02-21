function Start-WindowsUpdateInstallJob {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Private function called after ShouldProcess is checked.')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.WUInstallJob])]
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline)]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel[]]
        $WindowsUpdate,

        [Parameter(Mandatory)]
        [AllowEmptyString()]
        [AllowNull()]
        [string]
        $JobName,

        [Parameter(Mandatory)]
        [string]
        $Command
    )
    begin {
        $AllWindowsUpdates = New-WindowsUpdateCollection
    }
    process {
        foreach ($Update in $WindowsUpdate) {
            [void]$AllWindowsUpdates.Add($Update.ComObject)
        }
    }
    end {
        if ($AllWindowsUpdates.Count -eq 0) {
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateInstallJob] No updates identified to install. No job created to install updates."
            return
        }
        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateInstallJob] Starting install job to install $($AllWindowsUpdates.Count) Windows Update(s)."
        $Installer = New-WindowsUpdateInstaller
        $Installer.Updates = $AllWindowsUpdates
        if ($Installer.RebootRequiredBeforeInstallation) {
            Write-Warning 'A reboot is required before updates can be installed.'
        }
        $Job = [KPBSD.PowerShell.WindowsUpdate.WUInstallJob]::new($Command, $JobName)
        $Job | Start-WindowsUpdateJob $Installer
    }
}