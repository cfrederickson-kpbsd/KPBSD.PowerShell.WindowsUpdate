function Uninstall-WindowsUpdate {
    # [OutputType([KPBSD.PowerShell.WindowsUpdate.WUUninstallJob])]
    [CmdletBinding(SupportsShouldProcess, DefaultParameterSetName = 'TitlePassThru')]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'TitlePassThru')]
        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'TitleAsJob')]
        [SupportsWildcards()]
        [string[]]
        $Title,

        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'IdPassThru')]
        [Parameter(Mandatory, Position = 0, ValueFromPipelineByPropertyName, ParameterSetName = 'IdAsJob')]
        [Guid[]]
        $UpdateId,

        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ParameterSetName = 'UpdatePassThru')]
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ParameterSetName = 'UpdateAsJob')]
        [KPBSD.PowerShell.WindowsUpdate.UpdateModel[]]
        $WindowsUpdate,

        [Parameter(ParameterSetName = 'TitlePassThru')]
        [Parameter(ParameterSetName = 'IdPassThru')]
        [Parameter(ParameterSetName = 'UpdatePassThru')]
        [switch]
        $PassThru,

        [Parameter(Mandatory, ParameterSetName = 'TitleAsJob')]
        [Parameter(Mandatory, ParameterSetName = 'IdAsJob')]
        [Parameter(Mandatory, ParameterSetName = 'UpdateAsJob')]
        [switch]
        $AsJob,

        [Parameter(DontShow)]
        [switch]
        $ProcessPipelineAsIndividualJobs
    )
    begin {
        Assert-RunAsAdmin -Operation 'uninstall Windows Updates' | Out-Null
        $SynchronousJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
    }
    process {
        # Using object array so that we can mock. Assigning to $WindowsUpdate casts to the model class.
        if ($PSCmdlet.ParameterSetName -like 'Title*') {
            [object[]]$ActualUpdates = Get-WindowsUpdate -Title $Title -IncludeInstalled
        }
        elseif ($PSCmdlet.ParameterSetName -like 'Id*') {
            [object[]]$ActualUpdates = Get-WindowsUpdate -UpdateId $UpdateId -IncludeInstalled
        }
        else {
            [object[]]$ActualUpdates = $WindowsUpdate
        }

        # Using a psobject list so that we can mock Get-WindowsUpdate for testing
        if ($ProcessPipelineAsIndividualJobs -or $null -eq $AcceptedUpdates) {
            $AcceptedUpdates = [System.Collections.Generic.List[psobject]]::new()
        }
        foreach ($wu in $ActualUpdates) {
            if (!$wu.IsInstalled) {
                $ex = [System.InvalidOperationException]::new('The Windows Update is not installed on the computer.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'NotInstalled',
                    [System.Management.Automation.ErrorCategory]::NotInstalled,
                    $wu
                )
                $er.ErrorDetails = "The Windows Update $wu is not installed on the computer."
                $PSCmdlet.WriteError($er)
                continue
            }
            if (!$wu.IsUninstallable)
            {
                $ex = [System.NotSupportedException]::new('The Windows Update does not support uninstallation.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'UninstallationNotSupported',
                    [System.Management.Automation.ErrorCategory]::InvalidOperation,
                    $wu
                )
                $er.ErrorDetails = "The Windows Update $wu does not support uninstallation."
                $PSCmdlet.WriteError($er)
                continue
            }
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Uninstall-WindowsUpdate] Update $wu is installed on the computer. $($wu.IsInstalled)"
            if ($PSCmdlet.ShouldProcess(
                "Uninstalling Windows Update $Update.",
                "Uninstall Windows Update $Update?",
                'Confirm: uninstall Windows Update'
            ))
            {
                Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Uninstall-WindowsUpdate] Update $wu confirmed for uninstallation."
                $AcceptedUpdates.Add($wu)
            }
        }

        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Uninstall-WindowsUpdate] <Process> Proceeding with $($AcceptedUpdates.Count) accepted Windows Updates to uninstall."
        if ($ProcessPipelineAsIndividualJobs -and $AcceptedUpdates.Count -gt 0) {
            $Job = Start-WindowsUpdateUninstallJob -WindowsUpdate $AcceptedUpdates -JobName $PSBoundParameters['JobName'] -Command $MyInvocation.Line
            if ($AsJob) {
                $Job
            }
            else {
                $SynchronousJobs.Add($Job)
            }
        }
    }
    end {
        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Uninstall-WindowsUpdate] <End> Proceeding with $($AcceptedUpdates.Count) accepted Windows Updates to uninstall."
        if (!$ProcessPipelineAsIndividualJobs -and $AcceptedUpdates.Count -gt 0) {
            $Job = Start-WindowsUpdateUninstallJob -WindowsUpdate $AcceptedUpdates -JobName $PSBoundParameters['JobName'] -Command $MyInvocation.Line
            if ($AsJob) {
                $Job
            }
            else {
                $Job | Invoke-SynchronousJob | Where-Object { $PassThru }
            }
        }
    }
}