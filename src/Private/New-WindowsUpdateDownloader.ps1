function New-WindowsUpdateDownloader {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType('System.__ComObject#{68f1c6f9-7ecc-4666-a464-247fe12496c3}')]
    [CmdletBinding()]
    param()
    begin {
        $ErrorActionPreference = 'Stop'
    }
    process {
        $UpdateSession = Get-WindowsUpdateSession
        $Downloader = $UpdateSession.CreateUpdateDownloader()
        $Downloader.ClientApplicationId = $ExecutionContext.SessionState.Module.Name
        $Downloader
    }
}