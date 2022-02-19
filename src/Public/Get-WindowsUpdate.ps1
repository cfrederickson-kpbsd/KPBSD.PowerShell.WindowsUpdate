function Get-WindowsUpdate {
    [CmdletBinding(DefaultParameterSetName = 'TitleSet', PositionalBinding = $false)]
    param(
        # Filter by title. Supports wildcards.
        [Parameter(Position = 0, ParameterSetName = 'TitleSet')]
        [Alias('Name')]
        [SupportsWildcards()]
        [ValidateNotNullOrEmpty()]
        [string[]]
        $Title,

        # Filter by UpdateId. Does not support wildcards.
        [Parameter(Mandatory, ParameterSetName = 'IdSet')]
        [string[]]
        $UpdateId,

        # Include hidden updates.
        [Parameter()]
        [switch]
        $IncludeHidden,

        # Filter by the type of resource being updated.
        [Parameter()]
        [ValidateSet('Software', 'Driver', 'All')]
        [string]
        $UpdateType = 'All',

        # Do not go online to search for updates.
        [Parameter()]
        [switch]
        $SearchOffline,

        # Set the server that is searched for updates.
        [Parameter()]
        [KPBSD.PowerShell.WindowsUpdate.ServerSelectionArgumentValidation()]
        [KPBSD.PowerShell.WindowsUpdate.ServerSelectionArgumentCompleter()]
        [string]
        $Server,

        # Return the Get-WindowsUpdate operation as a job.
        [Parameter()]
        [switch]
        $AsJob
    )
    begin {
        $SynchronousJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
        $WhatIfPreference = $false
        $ConfirmPreference= $false
    }
    process {
        try {
            $Job = [KPBSD.PowerShell.WindowsUpdate.PSSearchJob]::new($PSCmdlet.MyInvocation.Line, $null)
            [WildcardPattern[]]$TitleWildcards = $Title | ForEach-Object { [WildcardPattern]::Get($_, 'IgnoreCase') }
            $ClientFilterParameters = [KPBSD.PowerShell.WindowsUpdate.WindowsUpdateSearchParameters]::new($TitleWildcards, $UpdateId, $IncludeHidden)
            $ServerCriteria = $ClientFilterParameters.GetServerFilter()
            $Job.ClientFilterParameters = $ClientFilterParameters
            $Job.Criteria = $ServerCriteria
            $Job.Online = -not $SearchOffline
            if ($Server -as [KPBSD.PowerShell.WindowsUpdate.ServerSelection]) {
                $Job.ServerSelection = $Server
            }
            elseif ($Server) {
                $Job.ServerSelection = 'Others'
                $Job.ServiceId = $Server
            }

            $Searcher = New-WindowsUpdateSearcher
            $Job | Start-WindowsUpdateJob $Searcher | Out-Null

            if ($AsJob) {
                $Job
            }
            else {
                $SynchronousJobs.Add($Job)
            }
        }
        finally {
            if ($PSCmdlet.Stopping) {
                $SynchronousJobs | Where-Object 'JobState' -eq 'Running' | Stop-Job -ErrorAction Ignore
                $SynchronousJobs | Remove-Job -Force -ErrorAction Ignore
            }
        }
    }
    end {
        try {
            $TitlesNotMatched = [System.Collections.Generic.HashSet[string]]::new($Title.Count, [System.StringComparer]::OrdinalIgnoreCase)
            $UpdateIdsNotMatched = [System.Collections.Generic.HashSet[string]]::new($UpdateId.Count, [System.StringComparer]::OrdinalIgnoreCase)
            foreach ($T in $Title) {
                if (-not [WildcardPattern]::ContainsWildcardCharacters($T)) {
                    [void]$TitlesNotMatched.Add($T)
                }
            }
            foreach ($U in $UpdateId) {
                [void]$UpdateIdsNotMatched.Add($U)
            }

            $Results = $SynchronousJobs | Receive-Job -Wait
            foreach ($Result in $Results) {
                if ($Title.Count -gt 0) {
                    $TitleMatched = $false
                    foreach ($T in $Title) {
                        if ($Result.Title -like $T) {
                            $TitleMatched = $true
                            break
                        }
                    }
                    if (-not $TitleMatched) {
                        continue
                    }
                }
                if ($UpdateId.Count -gt 0 -and $UpdateId -NotContains $Result.UpdateId) {
                    continue
                }

                [void]$TitlesNotMatched.Remove($Result.Title)
                [void]$UpdateIdsNotMatched.Remove($Result.UpdateId)
                $Result
            }

            foreach ($TitleNotMatched in $TitlesNotMatched) {
                $PSCmdlet.WriteError(
                    (New-ItemNotFoundError -ResourceType 'WindowsUpdate' -IdentifierName 'Title' -Identifier $TitleNotMatched)
                )
            }
            foreach ($UpdateIdNotMatched in $UpdateIdsNotMatched) {
                $PSCmdlet.WriteError(
                    (New-ItemNotFoundError -ResourceType 'WindowsUpdate' -IdentifierName 'UpdateId' -Identifier $UpdateIdNotMatched)
                )
            }
        }
        finally {
            if ($PSCmdlet.Stopping) {
                $SynchronousJobs | Where-Object 'JobState' -eq 'Running' | Stop-Job -ErrorAction Ignore
                $SynchronousJobs | Remove-Job -Force -ErrorAction Ignore
            }
        }
    }
}