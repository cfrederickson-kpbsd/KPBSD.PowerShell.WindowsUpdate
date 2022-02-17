# Some operations require administrative privilages to run.
# We will determine if we're running as admin and fail or proceed differently
# according to the current state.

$script:IsRunningAsAdmin = $null
function Test-RunAsAdmin {
    [CmdletBinding()]
    param()
    process {
        if ($null -eq $script:IsRunningAsAdmin) {
            $Identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
            $Principal = [System.Security.Principal.WindowsPrincipal]::new($Identity)
            $script:IsRunningAsAdmin = $Principal.IsInRole('Administrator')
        }
        $script:IsRunningAsAdmin
    }
}