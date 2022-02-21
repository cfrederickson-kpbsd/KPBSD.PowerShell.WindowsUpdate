Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
    Describe 'Assert-RunAsAdmin' {
        Context 'As Error' {
            It 'fails when not admin' {
                Mock Test-RunAsAdmin { $false }
                {
                    Assert-RunAsAdmin -Operation 'test'
                    'Not halted'
                } | Should -Throw
            }
            It 'does not proceed when not admin' {
                Mock Test-RunAsAdmin { $false }
                $Result = $false
                try {
                    Assert-RunAsAdmin -Operation 'test'
                    $Result = 'Not halted'
                }
                catch {
                    # Ignore for the purpose of this test
                }
                $Result | Should -BeFalse
            }
            It 'throws a descriptive error' {
                Mock Test-RunAsAdmin { $false }
                $DescriptiveError = $null
                try {
                    Assert-RunAsAdmin -Operation 'test'
                }
                catch {
                    $DescriptiveError = $_
                }
                $DescriptiveError | Should -BeTrue -Because 'the error must not be null'
                $DescriptiveError.ErrorDetails | Should -BeTrue
                $DescriptiveError.ErrorDetails.RecommendedAction | Should -BeTrue
                $DescriptiveError.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::PermissionDenied)
                $DescriptiveError.FullyQualifiedErrorId | Should -BeLike '*Unauthorized*'
            }
        }
        Context 'As Warning' {
            It 'returns $true as admin' {
                Mock Test-RunAsAdmin { $true }
                Assert-RunAsAdmin -AsWarning -Operation 'test' -WarningAction SilentlyContinue | Should -BeTrue
            }
            It 'returns $false when not admin' {
                Mock Test-RunAsAdmin { $false }
                Assert-RunAsAdmin -AsWarning -Operation 'test' -WarningAction SilentlyContinue | Should -BeFalse
            }
            It 'writes a warning message' {
                Mock Test-RunAsAdmin { $false }
                Assert-RunAsAdmin -AsWarning -Operation 'test' -WarningAction SilentlyContinue -WarningVariable AssertRunAsAdminWarnings | Out-Null
                $AssertRunAsAdminWarnings.Count | Should -Be 1
            }
        }
    }
}