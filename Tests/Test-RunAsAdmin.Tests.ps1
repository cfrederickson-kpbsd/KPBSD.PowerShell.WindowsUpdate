Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

Describe 'Test-RunAsAdmin' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        BeforeAll {
            function Set-CachedTestRunAsAdmin {
                param(
                    [Nullable[bool]]$IsAdmin,

                    [Nullable[bool]]$IsPowerUser
                )
                process {
                    $TestRunAsAdmin = Get-Command 'Test-RunAsAdmin'
                    & ($TestRunAsAdmin.Module) { $script:IsRunningAsAdmin = $IsAdmin; $script:IsRunningAsPowerUser = $IsPowerUser }
                }
            }
        }
        Context 'as admin' {
            It 'returns one item' {
                Test-RunAsAdmin | Should -HaveCount 1
            }
            It 'returns [bool]' {
                Test-RunAsAdmin | Should -BeOfType System.Boolean
            }
            It 'uses the cached result' {
                Set-CachedTestRunAsAdmin -IsAdmin $true -IsPowerUser $true
                Test-RunAsAdmin | Should -BeExactly $true
                Set-CachedTestRunAsAdmin -IsAdmin $false -IsPowerUser $true
                Test-RunAsAdmin | Should -BeExactly $false
                # Restore to initial state
                Set-CachedTestRunAsAdmin -IsAdmin $null -IsPowerUser $null
            }
            It 'returns true only if admin' {
                Set-CachedTestRunAsAdmin -IsAdmin $false -IsPowerUser $true
                Test-RunAsAdmin | Should -BeExactly $false
                Set-CachedTestRunAsAdmin -IsAdmin $true -IsPowerUser $false
                Test-RunAsAdmin | Should -BeExactly $true
                Set-CachedTestRunAsAdmin -IsAdmin $true -IsPowerUser $true
                Test-RunAsAdmin | Should -BeExactly $true
                # Restore to initial state
                Set-CachedTestRunAsAdmin -IsAdmin $null -IsPowerUser $null
            }
        }
        Context '-AllowPowerUser' {
            It 'returns one item' {
                Test-RunAsAdmin -AllowPowerUser | Should -HaveCount 1
            }
            It 'returns [bool]' {
                Test-RunAsAdmin -AllowPowerUser | Should -BeOfType 'System.Boolean'
            }
            It 'uses the cached result' {
                Set-CachedTestRunAsAdmin -IsAdmin $true -IsPowerUser $true
                Test-RunAsAdmin -AllowPowerUser | Should -BeExactly $true
                Set-CachedTestRunAsAdmin -IsAdmin $false -IsPowerUser $false
                Test-RunAsAdmin -AllowPowerUser| Should -BeExactly $false
                # Restore to initial state
                Set-CachedTestRunAsAdmin -IsAdmin $null -IsPowerUser $null
            }
            It 'returns true when not admin if is power user' {
                Set-CachedTestRunAsAdmin -IsAdmin $false -IsPowerUser $true
                Test-RunAsAdmin -AllowPowerUser | Should -BeExactly $true
                Set-CachedTestRunAsAdmin -IsAdmin $false -IsPowerUser $false
                Test-RunAsAdmin -AllowPowerUser| Should -BeExactly $false
                # Restore to initial state
                Set-CachedTestRunAsAdmin -IsAdmin $null -IsPowerUser $null
            }
            It 'returns true if the user is admin, regardless of whether they are a power user' {
                Set-CachedTestRunAsAdmin -IsAdmin $true -IsPowerUser $true
                Test-RunAsAdmin -AllowPowerUser | Should -BeExactly $true
                # Restore to initial state
                Set-CachedTestRunAsAdmin -IsAdmin $null -IsPowerUser $null
            }
        }
    }
}