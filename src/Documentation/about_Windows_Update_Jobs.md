# Windows Update Jobs
## about_Windows_Update_Jobs

```
ABOUT TOPIC NOTE:
The first header of the about topic should be the topic name.
The second header contains the lookup name used by the help system.

IE:
# Some Help Topic Name
## SomeHelpTopicFileName

This will be transformed into the text file
as `about_SomeHelpTopicFileName`.
Do not include file extensions.
The second header should have no spaces.
```

# SHORT DESCRIPTION
The KPBSD.PowerShell.WindowsUpdate module includes a new job type that can be used to monitor the state of a
Windows Update search, download, installation, or uninstallation process.

```
ABOUT TOPIC NOTE:
About topics can be no longer than 80 characters wide when rendered to text.
Any topics greater than 80 characters will be automatically wrapped.
The generated about topic will be encoded UTF-8.
```

# LONG DESCRIPTION
The KPBSD.PowerShell.WindowsUpdate module includes a new job type that can be used to monitor the state of a
Windows Update search, download, installation, or uninstallation process. These jobs are available by using
the AsJob parameter when calling the respective function. The WindowsUpdate jobs can be identified by the
PSJobTypeName of 'WindowsUpdateJob' and support the standard Get-Job, Stop-Job, Remove-Job, Wait-Job, and
Receive-Job commands.

Internally each of these commands runs the respective WindowsUpdate job, so there is no performance
degradation by getting the result of a command as a job.

# EXAMPLES
```powershell
PS C:\> Get-WindowsUpdate -Server WindowsUpdate -AsJob

Id     Name            PSJobTypeName   State         HasMoreData     Location             Command
--     ----            -------------   -----         -----------     --------             -------
1      Job1            WindowsUpdateJ… Running       False           COMPUTER_01          $GetUpdatesJob = Get-Win…

PS C:\> Get-Job | Receive-Job -Wait -AutoRemoveJob

Title                                                                                           UpdateId                             IsDownloaded IsInstalled IsMandatory
-----                                                                                           --------                             ------------ ----------- -----------
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.852.0) 8b90b97d-5c4d-44c9-89a1-0260a178d00e True         False       False
```

This example demonstrates using a Windows Update job to run a search for Windows Updates in the background
without blocking the main thread while the work completes.

# SEE ALSO
Get-WindowsUpdate
Request-WindowsUpdate
Install-WindowsUpdate

# KEYWORDS
{{List alternate names or titles for this topic that readers might use.}}

- WindowsUpdateJob
- WUInstallJob
- WUDownloadJob
- WUSearchJob
