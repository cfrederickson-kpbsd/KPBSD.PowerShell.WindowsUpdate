---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Suspend-WindowsAutomaticUpdate

## SYNOPSIS
Pauses Windows Automatic Updates so that updates are not downloaded and installed by Automatic Updates.

## SYNTAX

```
Suspend-WindowsAutomaticUpdate [<CommonParameters>]
```

## DESCRIPTION
Pauses Windows Automatic Updates so that updates are not downloaded and installed by Automatic Updates.
There is no way to check ahead of time if Windows Automatic Updates are running or paused, so this command
will not report an error if you suspend Windows Automatic Updates when it is already suspended.

## EXAMPLES

### Example 1
```powershell
PS C:\> Suspend-WindowsAutomaticUpdate
```

This example demonstrates using the command to ensure that Windows Automatic Updates are not currently running.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
[Start-WindowsAutomaticUpdate](./Start-WindowsAutomaticUpdate.md)
[Resume-WindowsAutomaticUpdate](./Resume-WindowsAutomaticUpdate.md)