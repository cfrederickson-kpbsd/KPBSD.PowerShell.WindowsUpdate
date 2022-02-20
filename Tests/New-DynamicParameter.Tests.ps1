Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
    Describe 'New-DynamicParameter' {
        BeforeAll {
            $NewDynamicParameterParameters = @{
                Name = 'TestParameter'
                ParameterType = 'PseudoType'
                DefaultValue = 'SomeDefaultValue'
                Mandatory = $true
                Position = 13
                ValueFromPipeline = $true
                Alias = 'DebugParameter', 'TrialParameter'
                ArgumentCompleter = { 'Completed', 'Argument' }
                ArgumentValidation = [ValidateScript]::new({ throw 'Not valid' }), { throw 'Not valid' }, 'ValidateNotNull'
                Attributes = [PSDefaultValue]@{ Value = 'SomeDefaultValue' }
            }
            $DynamicParameter = New-DynamicParameter @NewDynamicParameterParameters
            $null = $DynamicParameter # get rid of unused parameter warning
        }
        It 'outputs one item' {
            $DynamicParameter | Should -HaveCount 1
        }
        It 'returns [System.Management.Automation.RuntimeDefinedParameter]' {
            $DynamicParameter | Should -BeOfType 'System.Management.Automation.RuntimeDefinedParameter'
        }
        It 'sets the parameter type' {
            $DynamicParameter = New-DynamicParameter -Name 'TestParameter' -Type 'string'
            $DynamicParameter.ParameterType | Should -Be ([string])
        }
        It 'sets the PSTypeName when the type is not found' {
            [PSTypeNameAttribute]$TypeName = $DynamicParameter.Attributes | Where-Object { $_ -is [PSTypeNameAttribute] }
            $TypeName.PSTypeName | Should -Be 'PseudoType'
        }
        It 'sets the parameter attribute' {
            [Parameter]$ParameterAttribute = $DynamicParameter.Attributes | Where-Object { $_ -is [Parameter] }
            $ParameterAttribute | Should -Not -BeNull
            $ParameterAttribute.Position | Should -Be 13
            $ParameterAttribute.Mandatory | Should -BeExactly $true
            $ParameterAttribute.ValueFromPipeline | Should -BeExactly $true
        }
        It 'sets aliases' {
            [Alias]$AliasAttribute = $DynamicParameter.Attributes | Where-Object { $_ -is [Alias] }
            $AliasAttribute.AliasNames | Should -Be @('DebugParameter', 'TrialParameter')
        }
        It 'sets completers' {
            $CompleterAttributes = $DynamicParameter.Attributes | Where-Object { $_ -is [ArgumentCompleter] }
            $CompleterAttributes | Should -HaveCount 1
        }
        It 'sets validators' {
            $ValidatorAttributes = $DynamicParameter.Attributes | Where-Object { $_ -is [System.Management.Automation.ValidateArgumentsAttribute] }
            $ValidatorAttributes | Should -HaveCount 3
        }
    }
}