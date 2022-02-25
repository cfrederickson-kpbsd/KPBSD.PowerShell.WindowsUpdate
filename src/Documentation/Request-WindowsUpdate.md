---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Request-WindowsUpdate

## SYNOPSIS
Downloads a Windows Update onto the computer.

## SYNTAX

### TitleSet (Default)
```
Request-WindowsUpdate [-Title] <String[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### TitlePassThruSet
```
Request-WindowsUpdate [-Title] <String[]> [-PassThru] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### UpdatePassThruSet
```
Request-WindowsUpdate -WindowsUpdate <UpdateModel[]> [-PassThru] [-ProcessPipelineAsIndividualJobs] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### UpdateSet
```
Request-WindowsUpdate -WindowsUpdate <UpdateModel[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### IdPassThruSet
```
Request-WindowsUpdate -UpdateId <Guid[]> [-PassThru] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### IdSet
```
Request-WindowsUpdate -UpdateId <Guid[]> [-AsJob] [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Makes a request to the Windows Update server to download one or more Windows Updates.

## EXAMPLES

### Example 1
```powershell
PS C:\> Request-WindowsUpdate -Title '*Windows Defender*'
```

This example demonstrates using the command to download any available updates for Windows Defender.

### Example 2
```powershell
PS C:\> Get-WindowsUpdate | Where-Object IsDownloaded -eq $false | Download-WindowsUpdate
```

This example demonstrates using the alias 'Download-WindowsUpdate' in the pipeline to request a download
for all Windows Updates available to the computer that are not already downloaded.

## PARAMETERS

### -AsJob
Returns a job representing the download operation instead of waiting for the download to complete synchronously.

```yaml
Type: SwitchParameter
Parameter Sets: TitleSet, UpdateSet, IdSet
Aliases:

Required: False
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
Returns the downloaded Windows Updates after each download has completed.

```yaml
Type: SwitchParameter
Parameter Sets: TitlePassThruSet, UpdatePassThruSet, IdPassThruSet
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProcessPipelineAsIndividualJobs
Creates a new job for each Windows Update piped into this cmdlet. For cases where processing further up
the pipeline may be slower, this could start the download of updates from further up the pipeline
sooner. Results will still not be piped out of the command until the end{} block is reached.

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
Identifies updates to download by title.

```yaml
Type: String[]
Parameter Sets: TitleSet, TitlePassThruSet
Aliases: Name

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -UpdateId
Identifies updates to download by UpdateId.

```yaml
Type: Guid[]
Parameter Sets: IdPassThruSet, IdSet
Aliases:

Required: True
Position: Named
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
One or more Windows Update(s) to be downloaded.

```yaml
Type: UpdateModel[]
Parameter Sets: UpdatePassThruSet, UpdateSet
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### KPBSD.PowerShell.WindowsUpdate.UpdateModel[]

### System.Guid[]

## OUTPUTS

### KPBSD.PowerShell.WindowsUpdate.UpdateModel

## NOTES

## RELATED LINKS
