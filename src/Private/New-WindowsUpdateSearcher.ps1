function New-WindowsUpdateSearcher {
    [CmdletBinding()]
    [OutputType('System.__ComObject#{04c6895d-eaf2-4034-97f3-311de9be413a}')]
    param()
    begin {
        $ErrorActionPreference = 'Stop'
    }
    process {
        $Session = Get-WindowsUpdateSession
        $Searcher = $Session.CreateUpdateSearcher()
        $Searcher.ClientApplicationID = $ExecutionContext.SessionState.Module.Name
        $Searcher
    }
}