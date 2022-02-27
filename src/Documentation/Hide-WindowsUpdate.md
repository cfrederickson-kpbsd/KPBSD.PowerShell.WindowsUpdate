---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Hide-WindowsUpdate

## SYNOPSIS
Prevents a Windows Update from being returned by default with Get-WindowsUpdate or being installed by Automatic Updates.

## SYNTAX

### TitleSet (Default)
```
Hide-WindowsUpdate [-Title] <String[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### IdSet
```
Hide-WindowsUpdate [-UpdateId] <Guid[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UpdateSet
```
Hide-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Marks a Windows Update as "Hidden", which prevents the update from being included by `Get-WindowsUpdate`
unless specifically requested, and prevents Automatic Updates from installing the update.

The same effect may be achieved by setting the IsHidden property of a Windows Update to `$true`. This
function offers better validation and error handling, along with pipeline support.

Updates that are mandatory or already hidden (IsHidden property is `$true`) cannot be hidden by this
command. This command requires elevated access to run. The user must be running PowerShell as an
administrator or PowerUser.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdate -Title '*Microsoft Defender*' -OutVariable MicrosoftDefenderUpdates

Title                                                                                           UpdateId                             IsDownloaded IsInstalled IsMandatory
-----                                                                                           --------                             ------------ ----------- -----------
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.852.0) 8b90b97d-5c4d-44c9-89a1-0260a178d00e True         False       False

PS C:\> $MicrosoftDefenderUpdates | Hide-WindowsUpdate
PS C:\> Get-WindowsUpdate -Title '*Microsoft Defender*'
# No results
```

This example demonstrates using the Get-WindowsUpdate command to identify a Windows Update, and then using
Hide-WindowsUpdate to prevent it from being returned in subsequent calls to Get-WindowsUpdate.

## PARAMETERS

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Write the Windows Update to the pipeline after hiding it. The update is not returned by the command
if it is not hidden by the command (any update that failed to be hidden will not be output).

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

### -Title
Identifies Windows Updates to hide by title.

```yaml
Type: String[]
Parameter Sets: TitleSet
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### -UpdateId
Identifies Windows Updates to hide by UpdateId.

```yaml
Type: Guid[]
Parameter Sets: IdSet
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WindowsUpdate
The Windows Update(s) to hide.

```yaml
Type: UpdateModel[]
Parameter Sets: UpdateSet
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### System.Guid[]

### KPBSD.PowerShell.WindowsUpdate.UpdateModel[]

## OUTPUTS

### KPBSD.PowerShell.WindowsUpdate.UpdateModel
A representation of a Windows Update.

## NOTES

## RELATED LINKS
[Show-WindowsUpdate](./Show-WindowsUpdate.md)
[Get-WindowsUpdate](./Get-WindowsUpdate.md)