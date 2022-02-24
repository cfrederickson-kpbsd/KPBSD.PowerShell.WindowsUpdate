Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1')

Describe 'Hide-WindowsUpdate' {
    BeforeAll {
        Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $true }
        Mock 'Get-WindowsUpdate' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' {
            @(
                [PSCustomObject]@{
                    Title = 'Visible Windows Update'
                    UpdateId = '91524386-81cf-47a3-b3c3-601c5e99f341'
                    IsHidden = $false
                    IsMandatory  = $false
                }
                [PSCustomObject]@{
                    Title = 'Hidden Windows Update'
                    UpdateId = '095e6083-6e3c-4111-9b6b-f3ad23ef1269'
                    IsHidden = $true
                    IsMandatory = $false
                }
                [PSCustomObject]@{
                    Title = 'Mandatory Windows Update'
                    UpdateId = '91524386-81cf-47a3-b3c3-601c5e99f341'
                    IsHidden = $false
                    IsMandatory  = $true
                }
            ) | Where-Object {
                (!$Title -or $Title -contains $_.Title) -and
                (!$UpdateId -or $UpdateId -contains $_.UpdateId)
            }
        }
    }
    It 'fails if the update is already hidden' {
        {
            Hide-WindowsUpdate -Title 'Hidden Windows Update' -ErrorAction Stop
        } | Should -Throw -ExceptionType 'System.InvalidOperationException' -ErrorId 'AlreadyHidden,Hide-WindowsUpdate'
    }
    It 'fails if the update is mandatory' {
        {
            Hide-WindowsUpdate -Title 'Mandatory Windows Update' -ErrorAction Stop
        } | Should -Throw -ExceptionType 'System.InvalidOperationException' -ErrorId 'MandatoryUpdate,Hide-WindowsUpdate'
    }
    It 'fails if not running as admin' {
        {
            Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $false }
            {
                Hide-WindowsUpdate -Title 'Visible Windows Update'
            } | Should -Throw -ExceptionType 'System.UnauthorizedAccessException' -ErrorId 'Unauthorized,Assert-RunAsAdmin'
        }
    }
    It 'returns nothing when -PassThru is $false' {
        Hide-WindowsUpdate -Title 'Visible Windows Update' | Should -HaveCount 0
    }
    It 'returns an update when -PassThru is $true' {
        Hide-WindowsUpdate -Title 'Visible Windows Update' -PassThru | Should -HaveCount 1
    }
    It 'does not PassThru if there is an error' {
        Hide-WindowsUpdate -Title 'Hidden Windows Update' -PassThru -ErrorAction Ignore | Should -HaveCount 0
    }
    It 'processes updates separately and will succeed for some even when others fail' {
        Hide-WindowsUpdate -Title 'Visible Windows Update', 'Hidden Windows Update' -ErrorAction Ignore -PassThru | Should -HaveCount 1
    }
    It 'sets IsHidden to $true' {
        $RevealedUpdate = Hide-WindowsUpdate -Title 'Visible Windows Update' -PassThru
        $RevealedUpdate.IsHidden | Should -BeExactly $true
    }
    It 'does not set IsHidden when -WhatIf is $true' {
        $PseudoUpdate = [PSCustomObject]@{
            Title = 'Visible Windows Update'
            UpdateId = '91524386-81cf-47a3-b3c3-601c5e99f341'
            IsHidden = $false
            IsMandatory  = $false
        }
        Mock 'Get-WindowsUpdate' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $PseudoUpdate }
        Hide-WindowsUpdate -Title 'Visible Windows Update' -WhatIf
        $PseudoUpdate.IsHidden | Should -BeExactly $false -Because 'IsHidden should not have changed'
        Hide-WindowsUpdate -Title 'Visible Windows Update'
        $PseudoUpdate.IsHidden | Should -BeExactly $true -Because 'IsHidden should now be different'
    }
}