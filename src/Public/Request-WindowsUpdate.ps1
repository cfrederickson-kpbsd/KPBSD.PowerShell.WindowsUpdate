function Request-WindowsUpdate {
    [Alias('Download-WindowsUpdate', 'dlwu', 'rqwu')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel])]
    [CmdletBinding(SupportsShouldProcess, PositionalBinding = $false, DefaultParameterSetName = 'TitleSet')]
    param(
        [Parameter(ParameterSetName = 'TitleSet', Mandatory, Position = 0)]
        [Parameter(ParameterSetName = 'TitlePassThruSet', Mandatory, Position = 0)]
        [SupportsWildcards()]
        [Alias('Name')]
        [string[]]
        $Title,

        [Parameter(ParameterSetName = 'UpdateSet', Mandatory, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'UpdatePassThruSet', Mandatory, ValueFromPipeline)]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel[]]
        $WindowsUpdate,

        [Parameter(ParameterSetName = 'IdSet', Mandatory, ValueFromPipelineByPropertyName)]
        [Parameter(ParameterSetName = 'IdPassThruSet', Mandatory, ValueFromPipelineByPropertyName)]
        [Guid[]]
        $UpdateId,

        [Parameter(ParameterSetName = 'TitleSet')]
        [Parameter(ParameterSetName = 'IdSet')]
        [Parameter(ParameterSetName = 'UpdateSet')]
        [switch]
        $AsJob,

        [Parameter(ParameterSetName = 'TitlePassThruSet', Mandatory)]
        [Parameter(ParameterSetName = 'IdPassThruSet', Mandatory)]
        [Parameter(ParameterSetName = 'UpdatePassThruSet', Mandatory)]
        [switch]
        $PassThru,

        [Parameter(DontShow)]
        [switch]
        $ProcessPipelineAsIndividualJobs
    )
    dynamicparam {
        if ($AsJob) {
            New-DynamicParameter -Name 'JobName' -Type 'string' | New-DynamicParameterDictionary
        }
    }
    begin {
        Assert-RunAsAdmin -Operation 'download Windows Updates' | Out-Null
        $SynchronousJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
    }
    process {
        if ($PSCmdlet.ParameterSetName -in @('TitleSet', 'TitlePassThruSet')) {
            $WindowsUpdate = Get-WindowsUpdate -Title $Title
        }
        if ($PSCmdlet.ParameterSetName -in @('IdSet', 'IdPassThruSet')) {
            $WindowsUpdate = Get-WindowsUpdate -UpdateId $UpdateId
        }
        if ($ProcessPipelineAsIndividualJobs -or $null -eq $UpdatesToDownload) {
            $UpdatesToDownload = [System.Collections.Generic.List[KPBSD.PowerShell.WindowsUpdate.UpdateModel]]::new()
        }

        foreach ($Update in $WindowsUpdate) {
            if ($Update.IsDownloaded) {
                $ex = [System.InvalidOperationException]::new('The Windows Update is already downloaded onto the computer.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'UpdateAlreadyDownloaded',
                    'ResourceExists',
                    $Update
                )
                $er.ErrorDetails = "The Windows Update $($Update) is already downloaded onto the computer."
                $PSCmdlet.WriteError($er)
                continue
            }
            if ($PSCmdlet.ShouldProcess(
                "Downloading Windows Update $Update.",
                "Download Windows Update $Update?",
                'Confirm: download windows update')) {
                $UpdatesToDownload.Add($Update)
            }
        }

        if ($ProcessPipelineAsIndividualJobs -and $UpdatesToDownload.Count -gt 0) {
            $Job = Start-WindowsUpdateDownloadJob -WindowsUpdates $UpdatesToDownload -JobName $PSBoundParameters['JobName'] -Command $PSCmdlet.MyInvocation.Line
            if ($AsJob) {
                $Job
            }
            else {
                Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Request-WindowsUpdate] Adding job $Job to synchronous download queue."
                $SynchronousJobs.Add($Job)
            }
        }
    }
    end {
        if (!$ProcessPipelineAsIndividualJobs -and $UpdatesToDownload.Count -gt 0) {
            $Job = Start-WindowsUpdateDownloadJob -WindowsUpdates $UpdatesToDownload -JobName $PSBoundParameters['JobName'] -Command $PSCmdlet.MyInvocation.Line
            if ($AsJob) {
                $Job
            }
            else {
                Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Request-WindowsUpdate] Adding job $Job to synchronous download queue."
                $SynchronousJobs.Add($Job)
            }
        }
        $SynchronousJobs | Invoke-SynchronousJob | Where-Object { $PassThru }
    }
}