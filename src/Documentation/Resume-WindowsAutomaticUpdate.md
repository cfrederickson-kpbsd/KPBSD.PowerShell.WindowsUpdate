---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Resume-WindowsAutomaticUpdate

## SYNOPSIS
Resumes the Windows Automatic Update service, if suspended.

## SYNTAX

```
Resume-WindowsAutomaticUpdate [<CommonParameters>]
```

## DESCRIPTION
Resumes the Windows Automatic Update service, if suspended. There is no way to tell if the
service is currently running, and this command will not fail if the service is already
running.

## EXAMPLES

### Example 1
```powershell
PS C:\> Resume-WindowsAutomaticUpdate
```

This example demonstrates using the Resume-WindowsAutomaticUpdate command to ensure that
Windows Automatic Updates are not suspended.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
[Suspend-WindowsAutomaticUpdate](./Suspend-WindowsAutomaticUpdate.md)
[Start-WindowsAutomaticUpdate](./Start-WindowsAutomaticUpdate.md)