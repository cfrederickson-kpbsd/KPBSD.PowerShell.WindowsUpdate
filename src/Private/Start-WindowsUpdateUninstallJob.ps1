function Start-WindowsUpdateUninstallJob {
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
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateUninstallJob] No updates identified to uninstall. No job created to uninstall updates."
            return
        }
        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateUninstallJob] Starting uninstall job to uninstall $($AllWindowsUpdates.Count) Windows Update(s)."
        $Installer = New-WindowsUpdateInstaller
        $Installer.Updates = $AllWindowsUpdates
        if ($Installer.RebootRequiredBeforeInstallation) {
            Write-Warning 'A reboot is required before updates can be installed.'
        }
        $Job = [KPBSD.PowerShell.WindowsUpdate.WUInstallJob]::new($Command, $JobName)
        $Job.IsUninstallation = $true
        $Job | Start-WindowsUpdateJob $Installer
    }
}