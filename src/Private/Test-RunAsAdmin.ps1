# Some operations require administrative privilages to run.
# We will determine if we're running as admin and fail or proceed differently
# according to the current state.

$script:IsRunningAsAdmin = $null
$script:IsRunningAsPowerUser = $null
function Test-RunAsAdmin {
    [OutputType([bool])]
    [CmdletBinding()]
    param(
        [Parameter()]
        [switch]
        $AllowPowerUser
    )
    process {
        if ($null -eq $script:IsRunningAsAdmin) {
            $Identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
            $Principal = [System.Security.Principal.WindowsPrincipal]::new($Identity)
            $script:IsRunningAsAdmin = $Principal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator)
            $script:IsRunningAsPowerUser = $Principal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::PowerUser)
        }
        $script:IsRunningAsAdmin -or ($AllowPowerUser -and $script:IsRunningAsPowerUser)
    }
}