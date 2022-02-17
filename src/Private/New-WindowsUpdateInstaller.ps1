function New-WindowsUpdateInstaller {
    [CmdletBinding()]
    [OutputType('System.__ComObject#{ef8208ea-2304-492d-9109-23813b0958e1}')]
    param()
    begin {
        $ErrorActionPreference = 'Stop'
    }
    process {
        $Session = Get-WindowsUpdateSession
        $Installer = $Session.CreateUpdateInstaller()
        $Installer.ClientApplicationID = $ExecutionContext.SessionState.Module.Name
        $Installer.AllowSourcePrompts = $false
        if (Assert-RunAsAdmin -AsWarning -Operation 'Force Quiet installer prompts' -Consequence 'script may hang') {
            $Installer.ForceQuiet = $true
        }
        $Installer
    }
}