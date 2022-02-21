function Start-WindowsUpdateDownloadJob {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Private function called after ShouldProcess is checked.')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.WUDownloadJob])]
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline)]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel[]]
        $WindowsUpdates,

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
        foreach ($Update in $WindowsUpdates) {
            [void]$AllWindowsUpdates.Add($Update.ComObject)
        }
    }
    end {
        if ($AllWindowsUpdates.Count -eq 0) {
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateDownloadJob] No updates identified to download. No job created to download updates."
            return
        }
        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Start-WindowsUpdateDownloadJob] Starting download job to download $($AllWindowsUpdates.Count) Windows Update(s)."
        $Downloader = New-WindowsUpdateDownloader
        $Downloader.Updates = $AllWindowsUpdates
        $Job = [KPBSD.PowerShell.WindowsUpdate.WUDownloadJob]::new($Command, $JobName)
        $Job | Start-WindowsUpdateJob $Downloader
    }
}