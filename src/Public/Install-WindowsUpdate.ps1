function Install-WindowsUpdate {
    [Alias('iswu')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel], ParameterSetName = 'TitlePassThru')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel], ParameterSetName = 'IdPassThru')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel], ParameterSetName = 'UpdatePassThru')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.WindowsUpdateJob], ParameterSetName = 'TitleAsJob')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.WindowsUpdateJob], ParameterSetName = 'IdAsJob')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.WindowsUpdateJob], ParameterSetName = 'UpdateAsJob')]
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

        [Parameter()]
        [ValidateSet('Ignore', 'Prompt', 'AcceptAll')]
        [string]
        $AcceptEulaBehavior = 'Ignore',

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
        Assert-RunAsAdmin -Operation 'install Windows Updates' | Out-Null
        $SynchronousJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
    }
    process {
        # Using object array so that we can mock. Assigning to $WindowsUpdate casts to the model class.
        if ($PSCmdlet.ParameterSetName -like 'Title*') {
            [object[]]$ActualUpdates = Get-WindowsUpdate -Title $Title
        }
        elseif ($PSCmdlet.ParameterSetName -like 'Id*') {
            [object[]]$ActualUpdates = Get-WindowsUpdate -UpdateId $UpdateId
        }
        else {
            [object[]]$ActualUpdates = $WindowsUpdate
        }

        # Using a psobject list so that we can mock Get-WindowsUpdate for testing
        if ($ProcessPipelineAsIndividualJobs -or $null -eq $AcceptedUpdates) {
            $AcceptedUpdates = [System.Collections.Generic.List[psobject]]::new()
        }
        foreach ($wu in $ActualUpdates) {
            if (!$wu.IsDownloaded) {
                $ex = [System.InvalidOperationException]::new('The Windows Update cannot be installed because it is not downloaded on the computer.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'NotDownloaded',
                    [System.Management.Automation.ErrorCategory]::InvalidData,
                    $wu
                )
                $er.ErrorDetails = "The Windows Update $wu cannot be installed because it is not downloaded on the computer."
                $er.ErrorDetails.RecommendedAction = 'Download the update using the Request-WindowsUpdate function (or alias Download-WindowsUpdate) and try again.'
                $PSCmdlet.WriteError($er)
                continue
            }
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] Update $wu is downloaded on the computer $($wu.IsDownloaded)."
            if ($wu.IsInstalled) {
                $ex = [System.InvalidOperationException]::new('The Windows Update is already installed on the computer.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'AlreadyInstalled',
                    [System.Management.Automation.ErrorCategory]::ResourceExists,
                    $wu
                )
                $er.ErrorDetails = "The Windows Update $wu is already installed on the computer."
                $PSCmdlet.WriteError($er)
                continue
            }
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] Update $wu is not installed on the computer. $($wu.IsInstalled)"
            if (!$wu.EulaAccepted)
            {
                switch ($AcceptEulaBehavior)
                {
                    'Ignore' {
                        $ex = [System.InvalidOperationException]::new('The EULA for the Windows Update has not been accepted.')
                        $er = [System.Management.Automation.ErrorRecord]::new(
                            $ex,
                            'EulaNotAccepted',
                            [System.Management.Automation.ErrorCategory]::MetadataError,
                            $wu
                        )
                        $er.ErrorDetails = "The EULA for the Windows Update $wu has not been accepted."
                        $er.ErrorDetails.RecommendedAction = 'To accept the EULA, run the command again with the AcceptEulaBehavior set to the appropriate behavior handling option.'
                        $PSCmdlet.WriteError($er)
                        break
                    }
                    'Prompt' {
                        $CancelOption = [System.Management.Automation.Host.ChoiceDescription]::new('&Cancel', 'Do not accept the license agreement. The installation for this update will be cancelled. Other updates may still be installed.')
                        $AcceptOption = [System.Management.Automation.Host.ChoiceDescription]::new('&Accept', 'Accept the license agreement and proceed with installation.')
                        $Accepted = $PSCmdlet.Host.UI.PromptForChoice("Accept EULA of Windows Update $wu", $wu.EulaText, @($CancelOption, $AcceptOption), 0)
                        if ($Accepted) {
                            $wu.ComObject.AcceptEula()
                        }
                        break
                    }
                    'AcceptAll' {
                        Write-Warning "By running this command you have accepted the terms to the following EULA for the Windows Update $wu.`n$($wu.EulaText)"
                        $wu.ComObject.AcceptEula()
                        break
                    }
                }
                if (!$wu.EulaAccepted) {
                    continue
                }
            }
            Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] Update $wu has accepted EULA. $($wu.EulaAccepted)"
            if ($PSCmdlet.ShouldProcess(
                "Installing Windows Update $Update.",
                "Install Windows Update $Update?",
                'Confirm: install Windows Update'
            ))
            {
                Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] Update $wu confirmed for installation."
                $AcceptedUpdates.Add($wu)
            }
        }

        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] <Process> Proceeding with $($AcceptedUpdates.Count) accepted Windows Updates to install."
        if ($ProcessPipelineAsIndividualJobs -and $AcceptedUpdates.Count -gt 0) {
            $StartWindowsUpdateJobParameters = @{
                'WindowsUpdate' = $AcceptedUpdates
                'JobName' = $PSBoundParameters['JobName']
                'Command' = $MyInvocation.Line
                'WindowsUpdateSession' = Get-WindowsUpdateSession
                'Install' = $true
            }
            $Job = Start-WindowsUpdateJob @StartWindowsUpdateJobParameters
            if ($AsJob) {
                $Job
            }
            else {
                $SynchronousJobs.Add($Job)
            }
        }
    }
    end {
        Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Install-WindowsUpdate] <End> Proceeding with $($AcceptedUpdates.Count) accepted Windows Updates to install."
        if (!$ProcessPipelineAsIndividualJobs -and $AcceptedUpdates.Count -gt 0) {
            $StartWindowsUpdateJobParameters = @{
                'WindowsUpdate' = $AcceptedUpdates
                'JobName' = $PSBoundParameters['JobName']
                'Command' = $MyInvocation.Line
                'WindowsUpdateSession' = Get-WindowsUpdateSession
                'Install' = $true
            }
            $Job = Start-WindowsUpdateJob @StartWindowsUpdateJobParameters
            if ($AsJob) {
                $Job
            }
            else {
                $Job | Invoke-SynchronousJob | Where-Object { $PassThru }
            }
        }
    }
}