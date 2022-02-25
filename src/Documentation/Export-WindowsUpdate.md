---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Export-WindowsUpdate

## SYNOPSIS
Exports a Windows Update to a folder.

## SYNTAX

```
Export-WindowsUpdate [-WindowsUpdate] <UpdateModel> [-OutputPath] <String> [-NoClobber] [-PassThru] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Copies the contents of a Windows Update to a folder. The update can then be re-imported using the
Import-WindowsUpdate function. Bundled updates must be exported individually.

Microsoft does not recommend using this function to share Windows Updates with other matchines.
For reference, see: https://docs.microsoft.com/en-us/previous-versions/windows/desktop/aa386101(v=vs.85)

To export a Windows Update from the update cache the update must first be downloaded. An error will
be reported if the update is not already downloaded.

## EXAMPLES

### Example 1
```powershell
PS C:\> Export-WindowsUpdate -WindowsUpdate $Update -OutputPath 'C:\WindowsUpdateCopies\2010-01-01' -NoClobber
```

This example demonstrates exporting a Windows Update to a directory on the local hard drive. If the output
directory of the update already exists, an error will be reported and no action will be taken.

### Example 2
```powershell
PS C:\> Get-WindowsUpdate -IncludeInstalled | Where-Object IsDownloaded | Export-WindowsUpdate -OutputPath {"C:\WindowsUpdateCopies\$($_.UpdateId)"}
```

This example uses the pipeline to export all downloaded Windows Updates on the computer that are still stored
in the update cache.

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

### -NoClobber
When preset, causes an error to be reported and no action to be taken if an item exists at the path specified
by 'OutputPath'.

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

### -OutputPath
A path to the directory in which the Windows Update will be exported. If the directory exists, all of its
contents will be removed and replaced with the Windows Update files unless -NoClobber is present.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PassThru
When present, the directory that the update was written to will be written to the pipelinle after the update has
been exported.

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
A Windows Update to be exported.

```yaml
Type: UpdateModel
Parameter Sets: (All)
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

### KPBSD.PowerShell.WindowsUpdate.UpdateModel

### System.String

## OUTPUTS

### System.IO.DirectoryInfo
When the PassThru parameter is present, the directory that the update was exported to will be written to the
pipeline.

## NOTES

## RELATED LINKS
https://docs.microsoft.com/en-us/previous-versions/windows/desktop/aa386101(v=vs.85)
Import-WindowsUpdate
Get-WindowsUpdate