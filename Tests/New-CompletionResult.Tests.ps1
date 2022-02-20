Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1')

Describe 'New-CompletionResult' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        It 'outputs one item' {
            New-CompletionResult 'CompletedValue' | Should -HaveCount 1
        }
        It 'outputs [System.Management.Automation.CompletionResult]' {
            New-CompletionResult 'CompletedValue' | Should -BeOfType 'System.Management.Automation.CompletionResult'
        }
        It 'accepts pipeline input' {
            'First', 'Second' | New-CompletionResult | Should -HaveCount 2
        }
        It 'filters to match the IfLike parameter' {
            'First', 'Second' | New-CompletionResult -IfLike 'Sec' | Should -HaveCount 1
        }
        It 'unquotes the IfLike parameter for comparison' {
            'First', 'Second' | New-CompletionResult -IfLike '''Sec' | Should -HaveCount 1
        }
        It 'adds a single quote if found in the IfLike parameter' {
            $Completion = 'First', 'Second' | New-CompletionResult -IfLike "'fir"
            $Completion.CompletionText | Should -Be '''First'''
        }
        It 'adds a double quote if found in the IfLike parameter' {
            $Completion = 'First', 'Second' | New-CompletionResult -IfLike '"fir'
            $Completion.CompletionText | Should -Be '"First"'
        }
        It 'adds quotes when the CompletionResult contains a space' {
            $Completion = New-CompletionResult "Argument Completion"
            $Completion.CompletionText | Should -Be "'Argument Completion'"
        }
        It 'does not add quotes when NoAutoQuoting is $true' {
            $Completion = New-CompletionResult "Argument Completion" -NoAutoQuoting
            $Completion.CompletionText | Should -Be 'Argument Completion'

            $Completion = New-CompletionResult "Argument Completion" -IfLike "'Argument" -NoAutoQuoting
            $Completion.CompletionText | Should -Be 'Argument Completion'
        }
    }
}