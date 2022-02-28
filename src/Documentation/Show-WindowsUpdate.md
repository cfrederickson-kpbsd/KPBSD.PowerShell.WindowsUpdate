---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Show-WindowsUpdate

## SYNOPSIS
Reveals a Windows Update so that it is no longer hidden, and is again able to be selected by default with
Get-WindowsUpdate or be installed by Automatic Updates.

## SYNTAX

### TitleSet (Default)
```
Show-WindowsUpdate [-Title] <String[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### IdSet
```
Show-WindowsUpdate [-UpdateId] <Guid[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UpdateSet
```
Show-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Marks a Windows Update as no longer hidden. When hidden, an update will not be includd by in a call to
`Get-WindowsUpdate` unless specifically requested, and Automatic Updates will not install the update.
Call this command on a hidden update to reveal the update so that it is no longer hidden.

The same effect may be achieved by setting the IsHidden property of a Windows Update to `$false`. This function
offers better validation and error handling, along with pipeline support.

Updates that are already visible (IsHidden property is `$false`) cannot be un-hidden by this command. This
command requires elevated access to run. The user must be running PowerShell as an administrator or PowerUser.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdate -Title '*Microsoft Defender*' -OutVariable MicrosoftDefenderUpdates -IncludeHidden | Select-Object -Property Title, UpdateId, IsHidden

Title                                                                                           UpdateId                             IsHidden    
-----                                                                                           --------                             ------------
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.852.0) 8b90b97d-5c4d-44c9-89a1-0260a178d00e True        

PS C:\> $MicrosoftDefenderUpdates | Show-WindowsUpdate -PassThru | Select-Object -Property Title, UpdateId, IsHidden

Title                                                                                           UpdateId                             IsHidden    
-----                                                                                           --------                             ------------
Security Intelligence Update for Microsoft Defender Antivirus - KB2267602 (Version 1.359.852.0) 8b90b97d-5c4d-44c9-89a1-0260a178d00e False        

```

This example demonstrates using the Show-WindowsUpdate command to reveal updates for Microsoft Defender that were previously hidden.

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
Writes the revealed Windows Update(s) to the pipeline after they have been updated to no longer be hidden.

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
Identifies Windows Updates to show by title.

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
Identifies Windows Updates to show by UpdateId.

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
Windows Updates to show.

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

## NOTES

## RELATED LINKS
[Get-WindowsUpdate](./Get-WindowsUpdate.md)
[Hide-WindowsUpdate](./Hide-WindowsUpdate.md)