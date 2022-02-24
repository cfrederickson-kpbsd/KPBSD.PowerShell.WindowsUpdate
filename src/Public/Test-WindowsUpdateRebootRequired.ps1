function Test-WindowsUpdateRebootRequired {
    [Alias('twurr')]
    [OutputType([bool])]
    [CmdletBinding()]
    param(

    )
    process {
        $SystemInfo = New-Object -ComObject Microsoft.Update.SystemInfo
        $SystemInfo.RebootRequired
    }
}