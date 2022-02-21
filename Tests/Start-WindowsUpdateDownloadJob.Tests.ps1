Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")

Describe 'Start-WindowsUpdateDownloadJob' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        It 'does not fail when given no jobs' {
            {
                Start-WindowsUpdateDownloadJob -WindowsUpdate @() -JobName 'Test' -Command 'Test' -ErrorAction Stop
            } | Should -Not -Throw
        }
        It 'has no output when given no jobs' {
            Start-WindowsUpdateDownloadJob -WindowsUpdate @() -JobName 'Test' -Command 'Test' | Should -HaveCount 0
        }
        It 'sets the job name' {
            Mock 'New-WindowsUpdateCollection' {
                [PSCustomObject]@{} | Add-Member -MemberType NoteProperty -Name Count -Value 1 -PassThru | Add-Member -MemberType ScriptMethod -Name Add -Value {} -PassThru
            }
            Mock 'New-WindowsUpdateDownloader' { [PSCustomObject]@{ Updates = '' } }
            Start-WindowsUpdateDownloadJob -WindowsUpdate @() -JobName 'Test' -Command 'Test' | Select-Object -ExpandProperty Name | Should -Be 'Test'
        }
        It 'sets the job command' {
            Mock 'New-WindowsUpdateCollection' {
                [PSCustomObject]@{} | Add-Member -MemberType NoteProperty -Name Count -Value 1 -PassThru | Add-Member -MemberType ScriptMethod -Name Add -Value {} -PassThru
            }
            Mock 'New-WindowsUpdateDownloader' { [PSCustomObject]@{ Updates = '' } }
            Start-WindowsUpdateDownloadJob -WindowsUpdate @() -JobName 'Test' -Command 'Test' | Select-Object -ExpandProperty Command | Should -Be 'Test'
        }
    }
}