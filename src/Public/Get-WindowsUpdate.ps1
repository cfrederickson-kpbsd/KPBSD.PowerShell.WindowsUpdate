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

        [Parameter()]
        [string[]]
        $CategoryName,

        [Parameter()]
        [string[]]
        $CategoryId,

        # Include hidden updates.
        [Parameter()]
        [switch]
        $IncludeHidden,

        [Parameter()]
        [switch]
        $IncludeInstalled,

        [Parameter()]
        [Nullable[KPBSD.PowerShell.WindowsUpdate.UpdateType]]
        $UpdateType,

        [Parameter()]
        [switch]
        $IsSearchForUninstallations,

        [Parameter()]
        [Nullable[bool]]
        $IsAssignedForAutomaticUpdates,

        [Parameter()]
        [Nullable[bool]]
        $BrowseOnly,

        [Parameter()]
        [Nullable[bool]]
        $AutoSelectOnWebSites,

        [Parameter()]
        [Nullable[bool]]
        $IsPresent,

        [Parameter()]
        [Nullable[bool]]
        $RebootRequired,

        [Parameter()]
        [switch]
        $IncludePotentiallySupersededUpdates,

        # Do not go online to search for updates.
        [Parameter()]
        [switch]
        $SearchOffline,

        # Set the server that is searched for updates.
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string]
        $Server = 'Default',

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
        $ConfirmPreference = $false

        $TitlesNotMatched = [System.Collections.Generic.HashSet[string]]::new($Title.Count, [System.StringComparer]::OrdinalIgnoreCase)
        $UpdateIdsNotMatched = [System.Collections.Generic.HashSet[string]]::new($UpdateId.Count, [System.StringComparer]::OrdinalIgnoreCase)
    }
    process {
        $ServiceId = $Server -as [Guid]
        if ($ServiceId)
        {
            $ServerSelection = 'Others'
        }
        else
        {
            # Use automation null to avoid casting null string to empty string
            $ServiceId = [System.Management.Automation.Internal.AutomationNull]::Value
            $ServerSelection = $Server -as [KPBSD.PowerShell.WindowsUpdate.ServerSelection]
            if (!$ServerSelection) {
                $ServerSelection = 'Default'
            }
        }
        $SearchJobFilter = [KPBSD.PowerShell.WindowsUpdate.SearchJobFilter]::new(
            <# bool includePotentiallySupersededUpdates, #> $IncludePotentiallySupersededUpdates,
            <# bool searchOffline, #> $SearchOffline,
            <# ServerSelection serverSelection, #> $ServerSelection,
            <# string? serviceId, #> $ServiceId,
            <# string[]? title, #> $Title,
            <# string[]? updateId, #> $UpdateId,
            <# string[]? categoryName, #> $CategoryName,
            <# string[]? categoryId, #> $CategoryId,
            <# bool includeHidden, #> $IncludeHidden,
            <# bool includeInstalled, #> $IncludeInstalled,
            <# UpdateType? type, #> $UpdateType,
            <# bool IsSearchForUninstallations, #> $IsSearchForUninstallations,
            <# bool? assignedForAutomaticUpdates, #> $IsAssignedForAutomaticUpdates,
            <# bool? browseOnly, #> $BrowseOnly,
            <# bool? autoSelectOnWebSites, #> $AutoSelectOnWebSites,
            <# bool? isPresent, #> $IsPresent,
            <# bool? rebootRequired #> $RebootRequired
        )
        $StartWindowsUpdateJobParameters = @{
            'Filter' = $SearchJobFilter
            'JobName' = $PSBoundParameters['JobName']
            'Command' = $MyInvocation.Line
            'WindowsUpdateSession' = Get-WindowsUpdateSession
        }
        $Job = Start-WindowsUpdateJob @StartWindowsUpdateJobParameters
        if ($AsJob) {
            $Job
        }
        else {
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
    end {
        if (!$AsJob) {
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
}