---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Start-WindowsAutomaticUpdate

## SYNOPSIS
Begins a Windows Automatic Update scan immediately.

## SYNTAX

```
Start-WindowsAutomaticUpdate [<CommonParameters>]
```

## DESCRIPTION
Begins a Windows Automatic Update scan immediately. The only way to monitor Windows Automatic Updates is by
checking the last scan time using `Get-WindowsAutomaticUpdateLastResults`.

## EXAMPLES

### Example 1
```powershell
PS C:\> Start-WindowsAutomaticUpdate
```

This example demonstrates using the Start-WindowsAutomaticUpdate command to prompt the Automatic Updates service
to begin checking for Windows Updates.

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
[Resume-WindowsAutomaticUpdate](./Resume-WindowsAutomaticUpdate.md)