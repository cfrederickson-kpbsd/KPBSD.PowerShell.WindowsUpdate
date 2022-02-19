function Get-WindowsUpdate {
    [CmdletBinding(DefaultParameterSetName = 'TitleSet', PositionalBinding = $false)]
    param(
        # Filter by title. Supports wildcards.
        [Parameter(Position = 0, ParameterSetName = 'TitleSet')]
        [Alias('Name')]
        [SupportsWildcards()]
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
    process {
        $Job = [KPBSD.PowerShell.WindowsUpdate.WindowsUpdateSearcherJob]::new($PSCmdlet.MyInvocation.Line, $null)
        $ClientFilterParameters = [KPBSD.PowerShell.WindowsUpdate.WindowsUpdateSearchParameters]::new($Title, $UpdateId, $IncludeHidden)
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

        $PSCmdlet.JobRepository.Add($Job)
        $Session = Get-WindowsUpdateSession
        $Job.StartJob($Session)

        if ($AsJob) {
            $Job
        }
        else {
            try {
                [string[]]$TitlesToMatch = @($Titles | Where-Object { -not [WildcardPattern]::ContainsWildcardCharacters($_) })
                $TitlesNotMatched = [System.Collections.Generic.HashSet[string]]::new($TitlesToMatch, [System.StringComparer]::OrdinalIgnoreCase)
                $UpdateIdsNotMatched = [System.Collections.Generic.HashSet[string]]::new($UpdateId, [System.StringComparer]::OrdinalIgnoreCase)
                $Results = $Job | Receive-Job -Wait
                foreach ($Result in $Results) {
                    if ($Title.Length -gt 0) {
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
                    if ($UpdateId -NotContains $Result.UpdateId) {
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
                if ($Job.State -eq 'Running') {
                    $Job | Stop-Job -ErrorAction Ignore -WhatIf:$false -Confirm:$false
                }
                $Job | Remove-Job -Force -WhatIf:$false -Confirm:$false -ErrorAction Ignore
            }
        }
    }
}