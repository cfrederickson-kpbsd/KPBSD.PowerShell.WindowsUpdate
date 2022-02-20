function New-ItemNotFoundError {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType([System.Management.Automation.ErrorRecord])]
    [CmdletBinding()]
    param(
        # The resource that was not found.
        [string]$ResourceType,

        # The property name that was matched against.
        [string]$IdentifierName,

        # The value that was matched against.
        [string]$Identifier
    )
    process {
        $Exception = [System.Management.Automation.ItemNotFoundException]::new("The $ResourceType with $IdentifierName $Identifier was not found.")
        $ErrorRecord = [System.Management.Automation.ErrorRecord]::new(
            $Exception,
            "$IdentifierName`NotFound",
            'ObjectNotFound',
            $Identifier
        )
        $ErrorRecord
    }
}