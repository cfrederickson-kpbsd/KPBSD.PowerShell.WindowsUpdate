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
        $PSDefaultParameterValues['*:WhatIf'] = $false
        $PSDefaultParameterValues['*:Confirm'] = $false
        try {
            Receive-Job $AllJobs -Wait -AutoRemoveJob
        }
        finally {
            $PSDefaultParameterValues['*:ErrorAction'] = 'Ignore'
            # $AllJobs | Where-Object 'State' -eq 'Running' | Stop-Job
            # $AllJobs | Remove-Job -Force
        }
    }
}