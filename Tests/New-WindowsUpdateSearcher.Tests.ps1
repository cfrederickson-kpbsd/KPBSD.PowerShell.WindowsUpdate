Describe 'New-WindowsUpdateSearcher' {
    It 'returns ''System.__ComObject#{04c6895d-eaf2-4034-97f3-311de9be413a}''' {
        $Searcher = New-WindowsUpdateSearcher
        $Searcher.PSTypeNames | Should -Contain 'System.__ComObject#{04c6895d-eaf2-4034-97f3-311de9be413a}'
    }
    It 'sets ClientApplicationID' {
        $Searcher = New-WindowsUpdateSearcher
        $Searcher.ClientApplicationID | Should -BeTrue
    }
}