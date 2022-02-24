$script:WindowsUpdateSession = $null

function Get-WindowsUpdateSession {
    [CmdletBinding()]
    [OutputType('System.__ComObject#{918efd1e-b5d8-4c90-8540-aeb9bdc56f9d}')]
    param()
    begin {
        $ErrorActionPreference = 'Stop'
    }
    process {
        if ($null -eq $script:WindowsUpdateSession) {
            $script:WindowsUpdateSession = New-Object -ComObject 'Microsoft.Update.Session'
            $script:WindowsUpdateSession.ClientApplicationID = $ExecutionContext.SessionState.Module.Name
        }
        $script:WindowsUpdateSession
    }
}