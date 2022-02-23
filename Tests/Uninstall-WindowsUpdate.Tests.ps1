Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1')

Describe 'Uninstall-WindowsUpdate' {
    BeforeAll {
        Mock 'Get-WindowsUpdate' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith {
            [CmdletBinding()]
            param(
                [string[]]
                $Title,

                [Guid[]]
                $UpdateId,

                [switch]
                $IncludeInstalled
            )
            @(
                [pscustomobject]@{
                    Title = 'Pending Windows Update'
                    RequiresManualInstallation = $false
                    InstalledForAnotherProduct = $true
                    UpdateId = '20c683a1-0c1a-4a1a-984a-2cfba9272188'
                    RevisionNumber = 200
                    EulaAccepted = $true
                    IsDownloaded = $false
                    IsInstalled = $false
                    IsUninstallable = $true
                    ComObject = [PSCustomObject]@{}
                }
                [pscustomobject]@{
                    Title = 'Installed Windows Update'
                    RequiresManualInstallation = $false
                    InstalledForAnotherProduct = $true
                    UpdateId = 'd285249a-f3c2-40d7-afa7-c1bbb52bdb15'
                    RevisionNumber = 200
                    EulaAccepted = $true
                    IsDownloaded = $true
                    IsInstalled = $true
                    IsUninstallable = $true
                    ComObject = [PSCustomObject]@{}
                }
                [pscustomobject]@{
                    Title = 'Permanent Windows Update'
                    RequiresManualInstallation = $true
                    InstalledForAnotherProduct = $false
                    UpdateId = 'c49d7d2f-d236-4bd5-90e6-dafeeb6fae30'
                    RevisionNumber = 200
                    EulaAccepted = $false
                    IsDownloaded = $true
                    IsInstalled = $true
                    IsUninstallable = $false
                    ComObject = [PSCustomObject]@{}
                }
            ) | Where-Object {
                $_.ComObject | Add-Member -MemberType ScriptMethod -Name AcceptEula -Value { $this.EulaAccepted = $true }

                if ($UpdateId.Count -gt 0) {
                    return $UpdateId -contains $_.UpdateId
                }
                elseif ($Title.Count -gt 0) {
                    foreach ($t in $title) {
                        if ($_.Title -like $t) {
                            return $true
                        }
                    }
                    return $false
                }
                else {
                    return $true
                }
            }
        }
    }
    Context 'mock to PassThru identified updates' {
        BeforeAll {
            Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $true }
            Mock 'Start-WindowsUpdateUninstallJob' -RemoveParameterType 'WindowsUpdate' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith {
                param($WindowsUpdate, $JobName, $Command)
                process { Start-Job { $using:WindowsUpdate } -Name $JobName }
            }
        }
        It 'identifies an update via Get-WindowsUpdate -Title' {
            Uninstall-WindowsUpdate -Title 'Installed Windows Update' -PassThru | Select-Object -ExpandProperty Title | Should -Be 'Installed Windows Update'
        }
        It 'identifies an update via Get-WindowsUpdate when passed -UpdateId' {
            Uninstall-WindowsUpdate -UpdateId 'd285249a-f3c2-40d7-afa7-c1bbb52bdb15' -PassThru | Select-Object -ExpandProperty UpdateId | Should -Be 'd285249a-f3c2-40d7-afa7-c1bbb52bdb15'
        }
        It 'does not output an update that it did not install' {
            Uninstall-WindowsUpdate -UpdateId 'c49d7d2f-d236-4bd5-90e6-dafeeb6fae30' -ErrorAction Ignore -PassThru | Should -HaveCount 0
        }
    }
    
    It 'fails if not running as admin' {
        Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $false }
        {
            Uninstall-WindowsUpdate -Title *
        } | Should -Throw
    }
    It 'fails if the update is not installed' {
        Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $true }
        Mock 'Start-WindowsUpdateUninstallJob' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { }
        {
            Uninstall-WindowsUpdate -UpdateId '20c683a1-0c1a-4a1a-984a-2cfba9272188' -ErrorAction Stop
        } | Should -Throw -ExceptionType System.InvalidOperationException -ErrorId 'NotInstalled,Uninstall-WindowsUpdate'
    }
    It 'fails if the update does not support uninstallation' {
        Mock 'Test-RunAsAdmin' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { $true }
        Mock 'Start-WindowsUpdateUninstallJob' -ModuleName 'KPBSD.PowerShell.WindowsUpdate' -MockWith { }
        {
            Uninstall-WindowsUpdate -UpdateId 'c49d7d2f-d236-4bd5-90e6-dafeeb6fae30' -ErrorAction Stop
        } | Should -Throw -ExceptionType System.NotSupportedException -ErrorId 'UninstallationNotSupported,Uninstall-WindowsUpdate'
    }
}