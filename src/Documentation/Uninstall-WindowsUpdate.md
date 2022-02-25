---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Uninstall-WindowsUpdate

## SYNOPSIS
Uninstalls one or more Windows Updates from the computer.

## SYNTAX

### TitlePassThru (Default)
```
Uninstall-WindowsUpdate [-Title] <String[]> [-PassThru] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### TitleAsJob
```
Uninstall-WindowsUpdate [-Title] <String[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### IdAsJob
```
Uninstall-WindowsUpdate [-UpdateId] <Guid[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### IdPassThru
```
Uninstall-WindowsUpdate [-UpdateId] <Guid[]> [-PassThru] [-ProcessPipelineAsIndividualJobs] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### UpdateAsJob
```
Uninstall-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### UpdatePassThru
```
Uninstall-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-PassThru] [-ProcessPipelineAsIndividualJobs]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Uninstalls a Windows Update that is installed on the computer. Not all Windows Updates can be uninstalled.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdate -IncludeInstalled -Title '*Microsoft Defender*' | Uninstall-WindowsUpdate
```

This command identifies any Windows Updates that were previously installed for Microsoft Defender
and uninstalls them. This command assumes that the caller has already determined that the only
updates identified by `Get-WindowsUpdate` are currently installed and are uninstallable.

## PARAMETERS

### -AsJob
Returns a job representing the uninstallation instead of blocking until the uninstallation has completed.

```yaml
Type: SwitchParameter
Parameter Sets: TitleAsJob, IdAsJob, UpdateAsJob
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Writes the Windows Update(s) to the pipeline after uninstallation has completed.

```yaml
Type: SwitchParameter
Parameter Sets: TitlePassThru, IdPassThru, UpdatePassThru
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProcessPipelineAsIndividualJobs
Processes each piped input as an individual job. This does not work for installation or uninstallation operations.

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
Identifies Windows Updates for uninstallation by title.

```yaml
Type: String[]
Parameter Sets: TitlePassThru, TitleAsJob
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### -UpdateId
Identifies Windows Updates for uninstalltion by UpdateId.

```yaml
Type: Guid[]
Parameter Sets: IdAsJob, IdPassThru
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
One or more Windows Updates to uninstall.

```yaml
Type: UpdateModel[]
Parameter Sets: UpdateAsJob, UpdatePassThru
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

### KPBSD.PowerShell.WindowsUpdate.WUInstallJob

## NOTES

## RELATED LINKS
