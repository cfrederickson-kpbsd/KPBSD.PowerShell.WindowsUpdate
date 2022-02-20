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
        $WhatIfPreference = $ConfirmPreference = $false
        try {
            while ($AllJobs.Count -gt 0) {
                Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Invoke-SynchronousJob] Waiting for $($AllJobs.Count) remaining job(s)."
                $CompletedJob = Wait-Job -Job $AllJobs -Any
                Receive-Job -Job $CompletedJob -ErrorVariable ReceivedJobErrors
                [void]$AllJobs.Remove($CompletedJob)
                Remove-Job -Job $CompletedJob
            }
        }
        finally {
            $ErrorActionPreference = 'Ignore'
            $AllJobs | Where-Object 'State' -eq 'Running' | Stop-Job
            $AllJobs | Remove-Job -Force
        }
    }
}