---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Get-WindowsAutomaticUpdateLastResults

## SYNOPSIS
Displays the results from the last time that Automatic Updates ran on the current computer.

## SYNTAX

```
Get-WindowsAutomaticUpdateLastResults [<CommonParameters>]
```

## DESCRIPTION
Displays the results from the last time that Automatic Updates ran on the current computer.
Automatic Updates will only run if they are enabled on the computer and the service has not
been paused.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WindowsAutomaticUpdateLastResults

IsServiceEnabled            : True
LastSearchSuccessDate       : 2/24/2022 12:24:28 PM
LastInstallationSuccessDate : 2/23/2022 8:46:26 PM
```

This example demonstrates the output of the command. This can be used to determine when Windows Automatic Updates
was last run.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### KPBSD.PowerShell.WindowsUpdate.AutomaticUpdateLastResults
An object which contains information about whether the Automatic Updates service is enabled
and the last time it successfully ran.

## NOTES

## RELATED LINKS
[Start-WindowsAutomaticUpdate](./Start-WindowsAutomaticUpdate.md)
[https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb513699(v=vs.85)](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb513699(v=vs.85))