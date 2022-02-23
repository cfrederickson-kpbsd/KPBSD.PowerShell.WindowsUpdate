Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1')

Describe 'Test-WindowsUpdateRebootRequired' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        It 'returns one value' {
            Test-WindowsUpdateRebootRequired | Should -HaveCount 1
        }
        It 'returns [bool]' {
            Test-WindowsUpdateRebootRequired | Should -BeOfType 'System.Boolean'
        }
        It 'returns the same value as the registry test' {
            $RegPath = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update'
            $RegKey = 'RebootRequired'
            $RegTest = Get-ItemProperty -Path $RegPath -Name $RegKey -ErrorAction Ignore
            Test-WindowsUpdateRebootRequired | Should -BeExactly ([bool]$RegTest)
        }
    }
}