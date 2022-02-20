Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

Describe 'Test-RunAsAdmin' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        It 'outputs one result' {
            Test-RunAsAdmin | Should -HaveCount 1
        }
        It 'outputs [bool]' {
            Test-RunAsAdmin | Should -BeOfType System.Boolean
        }
        It 'uses the cached result' {
            $TestRunAsAdmin = Get-Command Test-RunAsAdmin
            & ($TestRunAsAdmin.Module) { $script:IsRunningAsAdmin = $true }
            Test-RunAsAdmin | Should -BeExactly $true
            & ($TestRunAsAdmin.Module) { $script:IsRunningAsAdmin = $false }
            Test-RunAsAdmin | Should -BeExactly $false
            # Restore to initial state
            & ($TestRunAsAdmin.Module) { $script:IsRunningAsAdmin = $null }
        }
    }
}