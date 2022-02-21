function Clear-WindowsUpdateDownloadCache {
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'High')]
    param()
    process {
        $WindowsUpdateService = Get-Service -Name 'wuauserv' -ErrorAction Stop
        $reason = [System.Management.Automation.ShouldProcessReason]::None
        if ($PSCmdlet.ShouldProcess(
            'Removing all downloaded Windows Update files. This will temporarily stop the ''Windows Update'' service (''wuauserv'').',
            'Remove all downloaded Windows Update files? This will temporarily stop the ''Windows Update'' service (''wuauserv'').',
            'Confirm: remove all downloaded Windows Update files?',
            [ref]$reason
        ))
        {
            $ErrorActionPreference = 'Stop'
            $ConfirmPreference = 'None'
            $WhatIfPreference = $false
            try
            {
                # Ensure the Windows Update Agent service has stopped before removing these files
                if ($WindowsUpdateService.Status -ne 'Stopped') {
                    Write-Verbose "Stopping windows service '$($WindowsUpdateService.DisplayName)' ($($WindowsUpdateService.Name))."
                    $WindowsUpdateService | Stop-Service
                }
                else
                {
                    Write-Warning "The windows service '$($WindowsUpdateService.DisplayName)' ($($WindowsUpdateService.Name)) was not running. The service is automatically started as part of the '$($PSCmdlet.MyInvocation.MyCommand.Name)' command, and will be started when the command completes."
                }
                $WindowsUpdateService | Wait-ServiceStatus 'Stopped' -Timeout ([TimeSpan]::FromSeconds(30)) -ErrorAction 'Stop'

                Get-ChildItem "$env:windir/SoftwareDistribution/DataStore" -Recurse -Force | ForEach-Object {
                    Write-Verbose "Removing file at path '$($_.FullName)'."
                    $_
                } | Remove-Item -Force -Recurse
            }
            finally
            {
                if ($WindowsUpdateService.Status -ne 'Running') {
                    Write-Verbose "Starting windows service '$($WindowsUpdateService.DisplayName)' ($($WindowsUpdateService.Name))."
                    $WindowsUpdateService | Start-Service
                }
            }
        }
        elseif ($reason -eq 'WhatIf')
        {
            if ($WindowsUpdateService.Status -ne 'Stopped') {
                $WindowsUpdateService | Stop-Service -WhatIf
            }
            Get-ChildItem "$env:windir/SoftwareDistribution/DataStore" -Recurse -Force | Remove-Item -WhatIf -Recurse
            if ($WindowsUpdateService.Status -ne 'Running') {
                $WindowsUpdateService | Start-Service -WhatIf
            }
        }
    }
}