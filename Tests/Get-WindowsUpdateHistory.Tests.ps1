$MockNewWindowsUpdateSearcher = {
    # We call GetTotalHistoryCount() and then page through the results of QueryHistory(_, _)
    # So we need to mock both methods
    $Searcher = [PSCustomObject]@{}
    $Searcher | Add-Member -MemberType ScriptMethod -Name 'GetTotalHistoryCount' -Value {3}
    $Searcher | Add-Member -MemberType ScriptMethod -Name 'QueryHistory' -Value {
        # [System.Diagnostics.CodeAnalysis.SuppressMessage('*', 'PSReviewUnusedParameter')]
        param($StartIndex, $Quantity)
        # The command can filter by:
        # - Title
        # - UpdateId
        # - Date (min, max)
        # - ServerSelection
        # - Operation
        [PSCustomObject]@{
            Title = 'Some Windows Update Package #1'
            UpdateId = 'c6e36491-901d-4385-8f0c-1302fb321a58'
            Date = [datetime]'2020-01-01 0:00:00'
            ServerSelection = [KPBSD.PowerShell.WindowsUpdate.ServerSelection]'ManagedServer'
            Operation = [KPBSD.PowerShell.WindowsUpdate.OperationType]'Installation'
        }
        [PSCustomObject]@{
            Title = 'Some Windows Update Package #2'
            UpdateId = 'b43132bb-0e1a-4685-aed3-51b43f60ed0c'
            Date = [datetime]'2020-02-01 0:00:00'
            ServerSelection = [KPBSD.PowerShell.WindowsUpdate.ServerSelection]'ManagedServer'
            Operation = [KPBSD.PowerShell.WindowsUpdate.OperationType]'Uninstallation'
        }
        [PSCustomObject]@{
            Title = 'Any Other Incremental Upgrade'
            UpdateId = '86fdd438-d106-4e17-bb7e-c1e89f010c3e'
            Date = [datetime]'2020-03-01 0:00:00'
            ServerSelection = [KPBSD.PowerShell.WindowsUpdate.ServerSelection]'WindowsUpdate'
            Operation = [KPBSD.PowerShell.WindowsUpdate.OperationType]'Installation'
        }
    }
    $Searcher
}


Describe 'Get-WindowsUpdateHistory' {
    Context 'general' {
        It 'returns ''System.__ComObject#{c2bfb780-4539-4132-ab8c-0a8772013ab6}''' {
            $UpdateHistory = Get-WindowsUpdateHistory
            $UpdateHistory[0].PSTypeNames | Should -Contain 'System.__ComObject#{c2bfb780-4539-4132-ab8c-0a8772013ab6}'
        }
    }
    Context 'filtering' {
        BeforeAll {
            Mock 'New-WindowsUpdateSearcher' $MockNewWindowsUpdateSearcher
        }
        It 'filters ''-like Title''' {
            $Updates = @(Get-WindowsUpdateHistory -Title 'Some Windows Update Package #1')
            $Updates | Should -HaveCount 1
            $Updates.Title | Should -Be 'Some Windows Update Package #1' -Because 'no wildcards should cause exact match'

            $Updates = @(Get-WindowsUpdateHistory -Title 'Some Windows Update Package #?')
            $Updates | Should -HaveCount 2
            $Updates.Title | Should -BeLike 'Some Windows Update Package #?' -Because 'wildcards should be matched'
        }
        It 'filters ''-eq UpdateId'' (when match)' {
            $Updates = @(Get-WindowsUpdateHistory -UpdateId '86fdd438-d106-4e17-bb7e-c1e89f010c3e')
            $Updates | Should -HaveCount 1
            $Updates.UpdateIdentity.UpdateId | Should -Be '86fdd438-d106-4e17-bb7e-c1e89f010c3e' -Because 'UpdateId should be exact match'
        }
        It 'filters ''-eq UpdateId'' (when not matched)' {
            $Updates = Get-WindowsUpdateHistory -ErrorAction Ignore -UpdateId '*'
            $Updates | Should -HaveCount 0 -Because 'wildcards should not be matched'
        }
        It 'filters by MinimumDate' {
            $Updates = @(Get-WindowsUpdateHistory -MinimumDate '2020-01-05 00:00:00')
            $Updates | Should -HaveCount 2

            $Updates = @(Get-WindowsUpdateHistory -MinimumDate '2020-02-05 00:00:00')
            $Updates | Should -HaveCount 1
        }
        It 'filters by MaximumDate' {
            $Updates = @(Get-WindowsUpdateHistory -MaximumDate '2020-01-05 00:00:00')
            $Updates | Should -HaveCount 1

            $Updates = @(Get-WindowsUpdateHistory -MaximumDate '2020-02-05 00:00:00')
            $Updates | Should -HaveCount 2
        }
        It 'filters by ServerSelection' {
            $Updates = @(Get-WindowsUpdateHistory -InstalledFromServer 'ManagedServer')
            $Updates | Should -HaveCount 2
            $Updates.ServerSelection | Sould -Be ([KPBSD.PowerShell.WindowsUpdate.ServerSelection]::ManagedServer)
        }
        It 'filters by Operation' {
            $Updates = @(Get-WindowsUpdateHistory -Type 'Installation')
            $Updates | Should -HaveCount 2
            $Updates.ServerSelection | Sould -Be ([KPBSD.PowerShell.WindowsUpdate.OperationType]::Installation)
        }
    }
    Context 'error reporting' {
        BeforeAll {
            Mock 'New-WindowsUpdateSearcher' $MockNewWindowsUpdateSearcher
        }
        It 'writes a descriptive error when the provided Title is not found' {
            $null = Get-WindowsUpdateHistory -ErrorAction SilentlyContinue -Title 'Fake Title - Not Found' -ErrorVariable TitleNotFoundAll
            $TitleNotFoundAll.Count | Should -Be 1
            $TitleNotFound = $TitleNotFoundAll[0]
            $TitleNotFound.TargetObject | Should -Be 'Fake Title - Not Found'
            $TitleNotFound.Exception | Should -BeOfType System.Management.Automation.ItemNotFoundException
            $TitleNotFound.CategoryInfo.Category | Should -Be 'ObjectNotFound'
            $TitleNotFound.Activity | Should -Be 'New-WindowsUpdateSearcher'
            $TitleNotFound.Reason | Should -Be 'ItemNotFoundException'
        }
        It 'writes an error when the provided UpdateId is not found' {
            $null = Get-WindowsUpdateHistory -ErrorAction SilentlyContinue -UpdateId 'Fake Id - Not Found' -ErrorVariable UpdateIdNotFoundAll
            $UpdateIdNotFoundAll.Count | Should -Be 1
            $UpdateIdNotFound = $UpdateIdNotFoundAll[0]
            $UpdateIdNotFound.TargetObject | Should -Be 'Fake Id - Not Found'
            $UpdateIdNotFound.Exception | Should -BeOfType System.Management.Automation.ItemNotFoundException
            $UpdateIdNotFound.CategoryInfo.Category | Should -Be 'ObjectNotFound'
            $UpdateIdNotFound.Activity | Should -Be 'New-WindowsUpdateSearcher'
            $UpdateIdNotFound.Reason | Should -Be 'ItemNotFoundException'
        }
    }
}