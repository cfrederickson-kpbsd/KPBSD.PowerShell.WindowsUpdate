#Requires -Module ThreadJob
Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'Build\Debug\KPBSD.PowerShell.WindowsUpdate.psm1')

Describe 'Invoke-SynchronousJob' {
    InModuleScope 'KPBSD.PowerShell.WindowsUpdate' {
        Context 'pipeline input' {
            It 'waits for a job to complete' {
                $Job = Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                $Job | Invoke-SynchronousJob
                $Job.State | Should -Be 'Completed'
            }
            It 'waits for many jobs to complete' {
                $Jobs = @(
                    Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                    Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                )
                $Jobs | Invoke-SynchronousJob
                $Jobs.State | Should -Be @('Completed', 'Completed')
            }
            It 'returns immediately when no jobs are provided' {
                $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
                @() | Invoke-SynchronousJob
                $stopwatch.elapsed.totalmilliseconds | Should -BeLessThan 200
            }
            It 'removes the job when finished' {
                $Job = Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                $Job | Invoke-SynchronousJob
                Get-Job -Id $Job.Id -ErrorAction Ignore | Should -HaveCount 0 -Because 'the job should no longer exist'
            }
            It 'returns the output of each job, in completion order' {
                $Jobs = @(
                    Start-ThreadJob -ScriptBlock { start-sleep -seconds 0.1 ; 1 }
                    Start-ThreadJob -ScriptBlock { 2 }
                )
                $Jobs | Invoke-SynchronousJob | Should -Be @(2, 1)
            }
            It 'exposes all job streams' {
                $Job = Start-ThreadJob -ScriptBlock {
                    Write-Progress -Id 1 -PercentComplete 0 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Debug 'Debug message'
                    Write-Progress -Id 1 -PercentComplete 25 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Verbose 'Verbose message'
                    Write-Progress -Id 1 -PercentComplete 40 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Information 'Information message'
                    Write-Progress -Id 1 -PercentComplete 55 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Warning 'Warning message'
                    Write-Progress -Id 1 -PercentComplete 70 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Error 'Error message'
                    Write-Progress -Id 1 -PercentComplete 85 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Output 'Output message'
                    Write-Progress -Id 1 -PercentComplete 100 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message' -Completed
                }
                $null = $Job | Invoke-SynchronousJob -ErrorAction SilentlyContinue -WarningAction SilentlyContinue -ErrorVariable ev -InformationVariable iv -OutVariable ov -WarningVariable wv
                $ov | Should -HaveCount 1
                $ev | Should -HaveCount 1
                $wv | Should -HaveCount 1
                $iv | Should -HaveCount 1
                # $vv | Should -HaveCount 1
                # $dv | Should -HaveCount 1
                # $pv | Should -HaveCount 1
                # TODO: any way to monitor Verbose, Debug, and Progress output?
            }
        }
        Context 'named parameter input' {
            It 'waits for a job to complete' {
                $Job = Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 1 }
                Invoke-SynchronousJob $Job
                $Job.State | Should -Be 'Completed'
            }
            It 'waits for many jobs to complete' {
                $Jobs = @(
                    Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                    Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                )
                Invoke-SynchronousJob $Jobs
                $Jobs.State | Should -Be @('Completed', 'Completed')
            }
            It 'returns immediately when no jobs are provided' {
                $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
                Invoke-SynchronousJob @()
                $stopwatch.elapsed.totalmilliseconds | Should -BeLessThan 200
            }
            It 'removes the job when finished' {
                $Job = Start-ThreadJob -ScriptBlock { Start-Sleep -Seconds 0.2 }
                Invoke-SynchronousJob $Job
                Get-Job -Id $Job.Id -ErrorAction Ignore | Should -HaveCount 0 -Because 'the job should no longer exist'
            }
            It 'returns the output of each job, in completion order' {
                $Jobs = @(
                    Start-ThreadJob -ScriptBlock { start-sleep -seconds 0.1 ; 1 }
                    Start-ThreadJob -ScriptBlock { 2 }
                )
                Invoke-SynchronousJob $Jobs | Should -Be @(2, 1)
            }
            It 'exposes all job streams' {
                $Job = Start-ThreadJob -ScriptBlock {
                    Write-Progress -Id 1 -PercentComplete 0 -Activity 'Writing to streams' -CurrentOperation 'Writing debug message'
                    Write-Debug 'Debug message'
                    Write-Progress -Id 1 -PercentComplete 15 -Activity 'Writing to streams' -CurrentOperation 'Writing verbose message'
                    Write-Verbose 'Verbose message'
                    Write-Progress -Id 1 -PercentComplete 30 -Activity 'Writing to streams' -CurrentOperation 'Writing information message'
                    Write-Information 'Information message'
                    Write-Progress -Id 1 -PercentComplete 40 -Activity 'Writing to streams' -CurrentOperation 'Writing warning message'
                    Write-Warning 'Warning message'
                    Write-Progress -Id 1 -PercentComplete 60 -Activity 'Writing to streams' -CurrentOperation 'Writing error message'
                    Write-Error 'Error message'
                    Write-Progress -Id 1 -PercentComplete 75 -Activity 'Writing to streams' -CurrentOperation 'Writing output  message'
                    Write-Output 'Output message'
                    Write-Progress -Id 1 -PercentComplete 100 -Activity 'Writing to streams' -Completed
                }
                $null = $Job | Invoke-SynchronousJob -ErrorAction SilentlyContinue -WarningAction SilentlyContinue -ErrorVariable ev -InformationVariable iv -OutVariable ov -WarningVariable wv
                $ov | Should -HaveCount 1
                $ev | Should -HaveCount 1
                $wv | Should -HaveCount 1
                $iv | Should -HaveCount 1
                # $vv | Should -HaveCount 1
                # $dv | Should -HaveCount 1
                # $pv | Should -HaveCount 1
                # TODO: any way to monitor Verbose, Debug, and Progress output?
            }
        }
    }
}