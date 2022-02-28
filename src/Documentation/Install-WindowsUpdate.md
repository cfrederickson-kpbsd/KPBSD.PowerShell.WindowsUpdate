---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Install-WindowsUpdate

## SYNOPSIS
Installs a Windows Update onto the computer.

## SYNTAX

### TitlePassThru (Default)
```
Install-WindowsUpdate [-Title] <String[]> [-AcceptEulaBehavior <String>] [-PassThru]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### TitleAsJob
```
Install-WindowsUpdate [-Title] <String[]> [-AcceptEulaBehavior <String>] [-AsJob]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### IdAsJob
```
Install-WindowsUpdate [-UpdateId] <Guid[]> [-AcceptEulaBehavior <String>] [-AsJob]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### IdPassThru
```
Install-WindowsUpdate [-UpdateId] <Guid[]> [-AcceptEulaBehavior <String>] [-PassThru]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UpdateAsJob
```
Install-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-AcceptEulaBehavior <String>] [-AsJob]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UpdatePassThru
```
Install-WindowsUpdate [-WindowsUpdate] <UpdateModel[]> [-AcceptEulaBehavior <String>] [-PassThru]
 [-ProcessPipelineAsIndividualJobs] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Installs a Windows Update onto the computer. The Windows Update must be downloaded before installation may
occur. This command will not download the Windows Update; use `Request-WindowsUpdate` (alias:
`Download-WindowsUpdate`) to download the update before using this command to install the update.

Installing Windows Updates may be a slow process. For this reason, the Install-WindowsUpdate command
supports the AsJob parameter to run the Windows Update installation in the background. However, the
installation will be cancelled if the PowerShell window is closed so it is important to wait for the
job to complete before closing the process.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsUpdate | Request-WindowsUpdate -PassThru | Install-WindowsUpdate
```

This example demonstrates utilizing the pipeline to download and install all available Windows Updates.

## PARAMETERS

### -AcceptEulaBehavior
Determines if EULAs for updates are always accepted, prompted for acceptance, or always ignored during
installation.

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: Ignore, Prompt, AcceptAll

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AsJob
Returns a job representing the installation operation.

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
Writes the update to the pipeline after it has been installed.

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
Creates a new job to represent the installation process of each update that is piped into this command.

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
Identifies updates to install by title.

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
Identifies updates to install by UpdateId.

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
The Windows Updates to install.

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
[Get-WindowsUpdate](./Get-WindowsUpdate.md)
[Uninstall-WindowsUpdate](./Uninstall-WindowsUpdate.md)
[Request-WindowsUpdate](./Request-WindowsUpdate.md)