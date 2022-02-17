Describe 'Get-WindowsUpdateSession' {
    It 'returns ''System.__ComObject#{918efd1e-b5d8-4c90-8540-aeb9bdc56f9d}''' {
        $Session = Get-WindowsUpdateSession
        $Session.PSTypeNames | Should -Contain 'System.__ComObject#{918efd1e-b5d8-4c90-8540-aeb9bdc56f9d}'
    }
    It 'sets ClientApplicationID' {
        $Session = Get-WindowsUpdateSession
        $Session.ClientApplicationID | Should -BeTrue
    }
    It 'is not readonly' {
        $Session = Get-WindowsUpdateSession
        $Session.ReadOnly | Should -BeFalse
    }
}