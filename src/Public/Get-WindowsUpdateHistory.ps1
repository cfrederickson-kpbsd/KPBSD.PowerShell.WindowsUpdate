function Get-WindowsUpdateHistory {
    [CmdletBinding(DefaultParameterSetName = 'TitleSet', PositionalBinding = $false)]
    [OutputType('System.__ComObject#{c2bfb780-4539-4132-ab8c-0a8772013ab6}')]
    param(
        [Parameter(ParameterSetName = 'TitleSet', Position = 0)]
        [SupportsWildcards()]
        [Alias('Name')]
        [string[]]
        $Title = @(),

        [Parameter(ParameterSetName = 'IdSet')]
        [string[]]
        $UpdateId = @(),

        [Parameter()]
        [DateTime]
        $MinimumDate = [DateTime]::MinValue,

        [Parameter()]
        [DateTime]
        $MaximumDate = [DateTime]::MaxValue,

        [Parameter()]
        [KPBSD.PowerShell.WindowsUpdate.ServerSelection]
        $InstalledFromServer = 'Default',

        [Parameter()]
        [ValidateSet('Installation', 'Uninstallation', 'All')]
        [string]
        $Type = 'All'
    )
    begin {
        #$ErrorActionPreference = 'Stop'
    }
    process {
        $Searcher = New-WindowsUpdateSearcher
        $TotalCount = $Searcher.GetTotalHistoryCount()
        Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update history length: $TotalCount"
        Write-Information "Update history length: $TotalCount"
        # Process in pages. Particularly in case we're running in the center of a pipeline
        # so that we don't buffer all results before output

        $OperationTypeId = $Type -as [KPBSD.PowerShell.WindowsUpdate.OperationType]
        $CurrentPageNumber = 0
        $PageSize = 25
        $UpdateIdsNotMatched = [System.Collections.Generic.HashSet[string]]::new([string[]]$UpdateId, [System.StringComparer]::OrdinalIgnoreCase)
        $TitlesToMatch = @($Title | Where-Object { -not [WildcardPattern]::ContainsWildcardCharacters($_) })
        $TitlesNotMatched = [System.Collections.Generic.HashSet[string]]::new([string[]]$TitlesToMatch, [System.StringComparer]::OrdinalIgnoreCase)

        while ($CurrentPageNumber * $PageSize -lt $TotalCount) {
            $WriteProgressParameters = @{
                Activity = 'Get-WindowsUpdateHistory'
                Status = "Getting updates $($CurrentPageNumber * $PageSize)-$([Math]::Max($CurrentPageNumber * $PageSize + $PageSize, $TotalCount)) out of $($TotalCount)."
                PercentComplete = [int][Math]::Max([Math]::Truncate(($CurrentPageNumber * $PageSize) / $TotalCount), 100)
            }
            Write-Progress @WriteProgressParameters

            $HistoryContents = $Searcher.QueryHistory($CurrentPageNumber * $PageSize, $PageSize)
            foreach ($history in $HistoryContents) {
                # -like $Title
                if ($Title) {
                    $matchTitle = $false
                    foreach ($T in $Title) {
                        if ($history.Title -like $T) {
                            $matchTitle = $true
                            break
                        }
                    }
                    if (-not $matchTitle) {
                        Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by Title."
                        continue
                    }
                }

                # -eq $UpdateId
                if ([bool]$UpdateId -and ($UpdateId -notcontains $history.UpdateIdentity.UpdateId)) {
                    Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by UpdateId."
                    continue
                }

                if ($history.Date -lt $MinimumDate) {
                    Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by MinimumDate."
                    continue
                }

                if ($history.Date -gt $MaximumDate) {
                    Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by MaximumDate."
                    continue
                }

                if ($InstalledFromServer -ne 'Default' -and ($history.ServerSelection -ne $InstalledFromServer)) {
                    Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by InstalledFromServer."
                    continue
                }

                if ([bool]$OperationTypeId -and $history.Operation -ne $OperationTypeId) {
                    Write-Debug "$(Get-Date -f 'HH:mm:ss.ffff') [Get-WindowsUpdateHistory] Update $($history.Title) filtered by Type."
                    continue
                }

                [void]$TitlesNotMatched.Remove($history.Title)
                [void]$UpdateIdsNotMatched.Remove($history.UpdateIdentity.UpdateId)
                $history
            }
            $CurrentPageNumber += 1
        }

        foreach ($t in $TitlesNotMatched) {
            $PSCmdlet.WriteError(
                (New-ItemNotFoundError -ResourceType 'WindowsUpdateHistory' -IdentifierName 'Title' -Identifier $t)
            )
        }
        foreach ($i in $UpdateIdsNotMatched) {
            $PSCmdlet.WriteError(
                (New-ItemNotFoundError -ResourceType 'WindowsUpdateHistory' -IdentifierName 'UpdateId' -Identifier $i)
            )
        }

        Write-Progress -Activity 'Get-WindowsUpdateHistory' -Completed
    }
}