function Wait-ServiceStatus {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [System.ServiceProcess.ServiceControllerStatus]
        $Status,

        [Parameter(ValueFromPipeline, Mandatory)]
        [System.ServiceProcess.ServiceController]
        $Service,

        [Parameter()]
        [timespan]$Timeout = [system.threading.timeout]::InfiniteTimeSpan
    )
    begin {
        $Stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    }
    process {
        while ($Service.Status -ne $Status)
        {
            try {
                $Service.WaitForStatus($Status, [timespan]::FromMilliseconds(200))
            }
            catch [System.ServiceProcess.TimeoutException] {
                if ($Timeout -ne [System.Threading.Timeout]::InfiniteTimeSpan -and $Stopwatch.Elapsed -gt $Timeout)
                {
                    $er = [System.Management.Automation.ErrorRecord]::new(
                        $_.Exception,
                        'TimeoutElapsed',
                        [System.Management.Automation.ErrorCategory]::OperationTimeout,
                        $Service
                    )
                    $PSCmdlet.WriteError($er)
                    break
                }
            }
        }
    }
}