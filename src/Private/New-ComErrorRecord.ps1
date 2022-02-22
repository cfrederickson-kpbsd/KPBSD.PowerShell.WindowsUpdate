function New-ComErrorRecord {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType([System.Management.Automation.ErrorRecord])]
    [CmdletBinding()]
    param(
        # The resource that was not found.
        [Parameter(Mandatory)]
        [Alias('ErrorCode')]
        [int]
        $HResult,

        # The exception to wrap.
        [Parameter()]
        [Exception]
        $Exception,

        # The object being processed when the error occurred
        [Parameter()]
        [object]
        $TargetObject
    )
    process {
        [KPBSD.PowerShell.WindowsUpdate.ComErrorCodes]::CreateErrorRecord($HResult, $Exception, $TargetObject)
    }
}