$script:WindowsUpdateAutoUpdateManager = $null

function Get-WindowsUpdateAutoUpdateManager {
    [CmdletBinding()]
    param(

    )
    process {
        if ($null -eq $script:WindowsUpdateAutoUpdateManager) {
            $script:WindowsUpdateAutoUpdateManager = New-Object -ComObject 'Microsoft.Update.AutoUpdate'
        }
        $script:WindowsUpdateAutoUpdateManager
    }
}