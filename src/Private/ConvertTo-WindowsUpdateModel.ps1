function ConvertTo-WindowsUpdateModel {
    [OutputType([KPBSD.PowerShell.WindowsUpdate.Model])]
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline, Position = 0)]
        [AllowEmptyCollection()]
        [psobject[]]
        $InputObject,

        [Parameter()]
        [ValidateNotNull()]
        [Type]
        $RequiredType = [object]
    )
    process {
        foreach ($item in $InputObject) {
            try {
                $Model = [KPBSD.PowerShell.WindowsUpdate.Model]::CreateModel($item)
                [System.Management.Automation.LanguagePrimitives]::ConvertTo($Model, $RequiredType)
            }
            catch [System.Management.Automation.MethodInvocationException] {
                $PSCmdlet.WriteError($_)
            }
        }
    }
}