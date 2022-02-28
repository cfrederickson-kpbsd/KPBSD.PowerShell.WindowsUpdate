---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Get-WindowsUpdateHistory

## SYNOPSIS
Retrieves a list of history items indicating the previous Windows Update activity on the computer.

## SYNTAX

### TitleSet (Default)
```
Get-WindowsUpdateHistory [[-Title] <String[]>] [-MinimumDate <DateTime>] [-MaximumDate <DateTime>]
 [-InstalledFromServer <ServerSelection>] [-Type <String>] [<CommonParameters>]
```

### IdSet
```
Get-WindowsUpdateHistory [-UpdateId <String[]>] [-MinimumDate <DateTime>] [-MaximumDate <DateTime>]
 [-InstalledFromServer <ServerSelection>] [-Type <String>] [<CommonParameters>]
```

## DESCRIPTION
Identifies and lists the Windows Update installation and uninstallation history on the computer.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdateHistory

Title                                                                                                            UpdateId                             Date                  Operation      ResultCode
-----                                                                                                            --------                             ----                  ---------      ----------
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.809.0)                  fff4d6d5-1d2a-460e-bb85-9b7ea3a865d2 2/24/2022 5:46:26 AM  Installation   Succeeded
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.784.0)                  fd4e7850-f517-4087-853d-2420a24adb7d 2/23/2022 10:28:53 PM Installation   Succeeded
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.781.0)                  a6a5fcf6-aba6-4bc8-9e63-6cfe1b7bd1ef 2/23/2022 8:15:42 PM  Uninstallation Succeeded
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.781.0)                  a6a5fcf6-aba6-4bc8-9e63-6cfe1b7bd1ef 2/23/2022 8:06:59 PM  Uninstallation Succeeded
# Additional results omitted for brevity
```

This example demonstrates using the Get-WindowsUpdateHistory command.

## PARAMETERS

### -InstalledFromServer
Filters by server that the Windows Update was installed from.

```yaml
Type: ServerSelection
Parameter Sets: (All)
Aliases:
Accepted values: Default, ManagedServer, WindowsUpdate, Others

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaximumDate
The latest point in time a Windows Update installation must have occurred to be included in the results list.

```yaml
Type: DateTime
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MinimumDate
The earliest point in time a Windows Update installation must have occurred to be included in the results list.

```yaml
Type: DateTime
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title
Filters results by title.

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

### -Type
Filters by the operation performed (installation or uninstallation).

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: Installation, Uninstallation, All

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UpdateId
Filters by the UpdateId of the update that was installed or uninstalled.

```yaml
Type: String[]
Parameter Sets: IdSet
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### KPBSD.PowerShell.WindowsUpdate.UpdateHistoryModel
A representation of an update operation that occurred on the computer.

## NOTES

## RELATED LINKS
[Get-WindowsUpdate](./Get-WindowsUpdate.md)
[Install-WindowsUpdate](./Install-WindowsUpdate.md)
[Uninstall-WindowsUpdate](./Uninstall-WindowsUpdate.md)