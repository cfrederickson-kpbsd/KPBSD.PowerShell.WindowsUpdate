Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
    Describe 'New-DynamicParameterDictionary' {
        BeforeAll {
            $DynamicParam1 = New-DynamicParameter -Name 'FirstParameter' -Position 0 -Mandatory
            $DynamicParam2 = New-DynamicParameter -Name 'SecondParameter' -Position 1 -Type 'switch'
            $DynamicParameters = $DynamicParam1, $DynamicParam2 | New-DynamicParameterDictionary
            $null = $DynamicParameters
        }
        It 'outputs one item' {
            $DynamicParameters | Should -HaveCount 1
        }
        It 'outputs System.Management.Automation.RuntimeDefinedParameterDictionary' {
            $DynamicParameters | Should -BeOfType 'System.Management.Automation.RuntimeDefinedParameterDictionary'
        }
        It 'outputs a dictionary with all input parameters' {
            $DynamicParameters.Values | Should -Be $DynamicParam1, $DynamicParam2
        }
        It 'gives a descriptive error when provided conflicting parameter names' {
            $DynamicParam3 = New-DynamicParameter -Name 'FirstParameter' -Position 2
            $null = $DynamicParam1, $DynamicParam2, $DynamicParam3 | New-DynamicParameterDictionary -ErrorAction SilentlyContinue -ErrorVariable TestErrors
            $TestErrors | Should -HaveCount 1
            [System.Management.Automation.ErrorRecord]$TestError = $TestErrors[0]
            $TestError.TargetObject | Should -Be $DynamicParam3
            $TestError.CategoryInfo.Category | Should -Be 'InvalidArgument'
            $TestError.CategoryInfo.Activity | Should -Be 'New-DynamicParameterDictionary'
            $TestError.CategoryInfo.Activity | Should -Be 'New-DynamicParameterDictionary'
        }
        It 'provides first parameters even when given conflicting parameter names' {
            $DynamicParam3 = New-DynamicParameter -Name 'FirstParameter' -Position 2
            $DynamicParameters = $DynamicParam1, $DynamicParam3, $DynamicParam2 | New-DynamicParameterDictionary -ErrorAction Ignore
            $DynamicParameters.Keys | Should -HaveCount 2
        }
    }
}