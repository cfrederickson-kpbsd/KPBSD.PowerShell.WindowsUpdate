<#
.SYNOPSIS
Runs a job as if it were a synchronous operation, ensuring that the job is cleared from the PSJobRepository
when the command completes or is terminated.
#>
function Invoke-SynchronousJob {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [AllowEmptyCollection()]
        [System.Management.Automation.Job[]]
        $Job
    )
    begin {
        $AllJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
    }
    process {
        $AllJobs.AddRange($Job)
    }
    end {
        if ($AllJobs.Count -eq 0) {
            return
        }
        $WhatIfPreference = $ConfirmPreference = $false
        try {
            Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Invoke-SynchronousJob] Waiting for $($AllJobs.Count) job to be run as synchronous operations."
            Receive-Job -Wait -Job $AllJobs
            # while ($AllJobs.Count -gt 0) {
            #     $CompletedJob = Wait-Job -Job $AllJobs -Any
            #     Receive-Job -Job $CompletedJob
            #     [void]$AllJobs.Remove($CompletedJob)
            #     Remove-Job -Job $CompletedJob
            # }
        }
        finally {
            $ErrorActionPreference = 'SilentlyContinue'
            Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Invoke-SynchronousJob] Removing sync jobs (Errors: $($Error.Count)). $($AllJobs)."
            $AllJobs | Where-Object 'State' -eq 'Running' | Stop-Job
            Wait-Job -Job $AllJobs | Out-Null
            Remove-Job -Force -Job $AllJobs
            Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Invoke-SynchronousJob] Sync jobs cleaned up (Errors: $($Error.Count)). $(Get-Job)"
        }
    }
}