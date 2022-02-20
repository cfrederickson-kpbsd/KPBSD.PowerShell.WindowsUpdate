Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) "Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1")
InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
    Describe 'New-WindowsUpdateCollection' {
        It 'returns one object' {
            $UpdateCollection = @(New-WindowsUpdateCollection)
            $UpdateCollection | Should -HaveCount 1
        }
        It 'returns ''System.__ComObject#{07f7438c-7709-4ca5-b518-91279288134e}''' {
            $UpdateCollection = New-WindowsUpdateCollection
            $UpdateCollection.PSTypeNames | Should -Contain 'System.__ComObject#{07f7438c-7709-4ca5-b518-91279288134e}'
        }
    }
}