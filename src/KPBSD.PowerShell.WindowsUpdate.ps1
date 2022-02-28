# Module import script. This contains PowerShell source code for the module outside of any function
# definitions: for example, registering argument completers or setting up module state.

# The problem I run into with completions for this module is that the actual Get-WindowsUpdate
# command (through no fault of my own) is slow, and there doesn't seem to be a way to make it
# quickly check some cached set of updates.

# Parameters that I could offer timely completions for:
# Get-WindowsUpdate -Server (ServerSelection or ServiceID)
# Get-WindowsUpdateHistory -Title -UpdateId

$script:NoCompletions = [System.Management.Automation.CompletionResult[]]::new(0)
$script:NullableBoolCompletion = {
    $Completions = @(
        [System.Management.Automation.CompletionResult]::new('$true', 'True', 'ParameterValue', 'True')
        [System.Management.Automation.CompletionResult]::new('$false', 'False', 'ParameterValue', 'False')
        [System.Management.Automation.CompletionResult]::new('$null', 'Null', 'ParameterValue', 'Null')
    )
    $pattern = $args[2]
    if (!$pattern) { $pattern = "" }
    if ($pattern.StartsWith('$')) { $pattern = $pattern.Substring(1) }
    $wc = "$pattern*"
    $results = $Completions | Where-Object ListItemText -like $wc
    if ($results) {
        return $results
    }
    else {
        return [System.Management.Automation.CompletionResult[]]::new(0)
    }
}
$script:ServerSelectionCompletion = {
    $WindowsUpdateSession = Get-WindowsUpdateSession
    $ServiceManager = $WindowsUpdateSession.CreateUpdateServiceManager()
    [Enum]::GetValues([KPBSD.PowerShell.WindowsUpdate.ServerSelection]) | New-CompletionResult -IfLike $args[2]
    $ServiceManager.Services | New-CompletionResult -CompletionText { $_.ServiceId } -ListItemText { $_.Name } -ToolTip { $_.Name } -IfLike $args[2]
}
$script:UpdateHistoryIdCompletion = {
    Get-WindowsUpdateHistory | New-CompletionResult -CompletionText { $_.UpdateId } -IfLike $args[2]
}
$script:UpdateHistoryTitleCompletion = {
    Get-WindowsUpdateHistory | New-CompletionResult -CompletionText { $_.Title } -IfLike $args[2]
}

Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'RebootRequired' -ScriptBlock $script:NullableBoolCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'IsPresent' -ScriptBlock $script:NullableBoolCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'AutoSelectOnWebSites' -ScriptBlock $script:NullableBoolCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'BrowseOnly' -ScriptBlock $script:NullableBoolCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'IsAssignedForAutomaticUpdates' -ScriptBlock $script:NullableBoolCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdate' -ParameterName 'Server' -ScriptBlock $script:ServerSelectionCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdateHistory' -ParameterName 'Title' -ScriptBlock $script:UpdateHistoryTitleCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdateHistory' -ParameterName 'UpdateId' -ScriptBlock $script:UpdateHistoryIdCompletion
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdateHistory' -ParameterName 'MinimumDate' -ScriptBlock { Get-Date | New-CompletionResult -IfLike $args[2] }
Register-ArgumentCompleter -CommandName 'Get-WindowsUpdateHistory' -ParameterName 'MaximumDate' -ScriptBlock { Get-Date | New-CompletionResult -IfLike $args[2] }