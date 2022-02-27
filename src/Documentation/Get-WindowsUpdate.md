---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Get-WindowsUpdate

## SYNOPSIS
Retrieves Windows Updates available to a computer.

## SYNTAX

### TitleSet (Default)
```
Get-WindowsUpdate [[-Title] <String[]>] [-IncludeHidden] [-IncludeInstalled] [-SearchOffline]
 [-Server <String>] [-AsJob] [<CommonParameters>]
```

### IdSet
```
Get-WindowsUpdate -UpdateId <String[]> [-IncludeHidden] [-IncludeInstalled] [-SearchOffline] [-Server <String>]
 [-AsJob] [<CommonParameters>]
```

## DESCRIPTION
Searches for Windows Updates that are available for the computer. This command can use WSUS or Windows Update
depending on the value of the Server parameter.

While the Windows Update API provides more filtering than this command, in general performance does not degrade
significantly with client-side filtering and it was deemed that a simple command interface was more desireable.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdate -Server WindowsUpdate

Title                                                 UpdateId                             IsDownloaded IsInstalled IsMandatory
-----                                                 --------                             ------------ ----------- -----------
Dell Inc. - Monitor - 1/27/2016 12:00:00 AM - 1.0.0.0 62eefe54-cf8d-4c90-a1a2-6b2da996a865 False        False       False
Dell Inc. - Monitor - 2/1/2018 12:00:00 AM - 1.0.0.0  aaf3213d-daf5-4342-938f-9d82223548e3 False        False       False
Dell Inc. - Monitor - 1/27/2016 12:00:00 AM - 1.0.0.0 33ea9224-4142-485f-86bd-c914bf8beb42 False        False       False
Intel - Net - 12.18.9.10                              bf0e658b-e069-487c-b024-6e5791a688b5 False        False       False
```

This example demonstrates using the Get-WindowsUpdate command to identify updates available to the local computer.
The Server parameter is set to 'WindowsUpdate' to indicate that the Windows Updates service should be queried for
updates instead of checking against a WSUS server, if available.

### Example 2
```powershell
PS C:\> Get-WindowsUpdate -IncludeInstalled -Title '*Feature Update*'

Title                                                                     UpdateId                             IsDownloaded IsInstalled IsMandatory
-----                                                                     --------                             ------------ ----------- -----------
Feature update to Windows 10 (business editions), version 1803, en-us x64 4768da14-574f-456b-8c74-bc7159f3ac4a True         True        False
Feature update to Windows 10 (business editions), version 1803, en-us     eb129661-9791-4527-aa1d-6dfdd022ee9d True         True        False
```

This example demonstrates using the Title parameter to filter the results. The IncludeInstalled parameter may be
used to include Windows Updates that have already been installed on the computer.

## PARAMETERS

### -AsJob
Returns a job representing the current operation instead of waiting for the Windows Update retrieval to complete.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IncludeHidden
Includes updates that have been marked as hidden (using the Hide-WindowsUpdate command or a similar tool).

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IncludeInstalled
Includes updates that are already installed on the computer.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SearchOffline
Indicates that the Windows Update agent should not go online to search for Windows Updates.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Server
The ServiceID of a Windows Update service registered on the computer, or a string indicating the default server
to check ('Default', 'MicrosoftUpdate' (which searches WSUS), or 'WindowsUpdate'). This parameter offers
tab-completion which may help identify possible values.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title
Filters updates by title. This parameter supports wildcards.

```yaml
Type: String[]
Parameter Sets: TitleSet
Aliases: Name

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -UpdateId
Filters updates by UpdateId. This parameter does not support wildcards.

```yaml
Type: String[]
Parameter Sets: IdSet
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### KPBSD.PowerShell.WindowsUpdate.UpdateModel
A representation of a Windows Update.

## NOTES

## RELATED LINKS
[Request-WindowsUpdate](./Request-WindowsUpdate.md)
[Install-WindowsUpdate](./Install-WindowsUpdate.md)
[Get-WindowsUpdateHistory](./Get-WindowsUpdateHistory.md)