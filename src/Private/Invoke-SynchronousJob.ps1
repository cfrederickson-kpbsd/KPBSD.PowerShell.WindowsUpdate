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
            # Calling Receive-Job on all jobs together allows any job results to be exported in the order they are
            # generated.
            Receive-Job -Wait -Job $AllJobs
        }
        finally {
            $ErrorActionPreference = 'SilentlyContinue'
            Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Invoke-SynchronousJob] Removing sync jobs."
            $AllJobs | Where-Object 'State' -eq 'Running' | Stop-Job
            Wait-Job -Job $AllJobs | Out-Null
            Remove-Job -Force -Job $AllJobs
        }
    }
}