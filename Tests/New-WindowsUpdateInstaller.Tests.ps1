Describe 'New-WindowsUpdateInstaller' {
    BeforeAll {
        $PSDefaultParameterValues['New-WindowsUpdateInstaller:WarningAction'] = 'SilentlyContinue'
    }
    It 'returns ''System.__ComObject#{ef8208ea-2304-492d-9109-23813b0958e1}''' {
        $WarningPreference = 'SilentlyContinue'
        $Installer = New-WindowsUpdateInstaller
        $Installer.PSTypeNames | Should -Contain 'System.__ComObject#{ef8208ea-2304-492d-9109-23813b0958e1}'
    }
    It 'sets ClientApplicationID' {
        $WarningPreference = 'SilentlyContinue'
        $Installer = New-WindowsUpdateInstaller
        $Installer.ClientApplicationID | Should -BeTrue
    }
    It 'is non-interactive' {
        $WarningPreference = 'SilentlyContinue'
        $Installer = New-WindowsUpdateInstaller
        $Installer.AllowSourcePrompts | Should -BeFalse
        if (Test-RunAsAdmin) {
            $Installer.ForceQuiet | Should -BeTrue
        }
    }
}