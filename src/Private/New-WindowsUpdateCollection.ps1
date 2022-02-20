function New-WindowsUpdateCollection {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType('System.__ComObject#{07f7438c-7709-4ca5-b518-91279288134e}')]
    [CmdletBinding()]
    param()
    process {
        New-Object -ComObject 'Microsoft.Update.UpdateColl'
    }
}