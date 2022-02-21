function Get-WindowsUpdate {
    [Alias('gwu')]
    [OutputType([KPBSD.PowerShell.WindowsUpdate.UpdateModel])]
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
        [Parameter(Mandatory, ParameterSetName = 'IdSet', ValueFromPipelineByPropertyName)]
        [string[]]
        $UpdateId,

        # Include hidden updates.
        [Parameter()]
        [switch]
        $IncludeHidden,

        [Parameter()]
        [switch]
        $IncludeInstalled,

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
    dynamicparam {
        if ($AsJob) {
            New-DynamicParameter -Name 'JobName' -Type 'String' | New-DynamicParameterDictionary
        }
    }
    begin {
        $SynchronousJobs = [System.Collections.Generic.List[System.Management.Automation.Job]]::new()
        $WhatIfPreference = $false
        $ConfirmPreference= $false

        $TitlesNotMatched = [System.Collections.Generic.HashSet[string]]::new($Title.Count, [System.StringComparer]::OrdinalIgnoreCase)
        $UpdateIdsNotMatched = [System.Collections.Generic.HashSet[string]]::new($UpdateId.Count, [System.StringComparer]::OrdinalIgnoreCase)
    }
    process {
        try {
            $JobName = $PSBoundParameters['JobName']
            $Job = [KPBSD.PowerShell.WindowsUpdate.WUSearchJob]::new($PSCmdlet.MyInvocation.Line, $JobName)
            [WildcardPattern[]]$TitleWildcards = $Title | ForEach-Object { if ($_) { [WildcardPattern]::Get($_, 'IgnoreCase') } }
            $ClientFilterParameters = [KPBSD.PowerShell.WindowsUpdate.WindowsUpdateSearchParameters]::new($TitleWildcards, $UpdateId, $IncludeHidden, $IncludeInstalled)
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
            $Job | Start-WindowsUpdateJob $Searcher | Where-Object { $AsJob }
            if (!$AsJob) {
                Write-Debug "$(Get-Date -Format 'HH:mm:ss.ffff') [Get-WindowsUpdate] Adding job $Job to synchronous search queue."
                $SynchronousJobs.Add($Job)
            }

            foreach ($T in $Title) {
                if (-not [WildcardPattern]::ContainsWildcardCharacters($T)) {
                    [void]$TitlesNotMatched.Add($T)
                }
            }
            foreach ($U in $UpdateId) {
                [void]$UpdateIdsNotMatched.Add($U)
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
        $SynchronousJobs | Invoke-SynchronousJob -OutVariable Updates
        foreach ($update in $Updates) {
            [void]$TitlesNotMatched.Remove($update.Title)
            [void]$UpdateIdsNotMatched.Remove($update.UpdateId)
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
}