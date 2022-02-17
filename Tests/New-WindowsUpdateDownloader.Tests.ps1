Describe 'New-WindowsUpdateDownloader' {
    It 'returns ''System.__ComObject#{68f1c6f9-7ecc-4666-a464-247fe12496c3}''' {
        $Downloader = New-WindowsUpdateDownloader
        $Downloader.PSTypeNames | Should -Contain 'System.__ComObject#{68f1c6f9-7ecc-4666-a464-247fe12496c3}'
    }
    It 'sets ClientApplicationID' {
        $Downloader = New-WindowsUpdateDownloader
        $Downloader.ClientApplicationID | Should -BeTrue
    }
}