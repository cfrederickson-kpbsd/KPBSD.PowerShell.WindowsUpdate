---
external help file: KPBSD.PowerShell.WindowsUpdate-help.xml
Module Name: KPBSD.PowerShell.WindowsUpdate
online version:
schema: 2.0.0
---

# Test-WindowsUpdateRebootRequired

## SYNOPSIS
Determines if Windows Updates require a system reboot before installation for one or more updates can proceed.

## SYNTAX

```
Test-WindowsUpdateRebootRequired [<CommonParameters>]
```

## DESCRIPTION
Determines if Windows Updates require a system reboot before installation for one or more updates can proceed.

## EXAMPLES

### Example 1
```powershell
PS C:\> Test-WindowsUpdateRebootRequired
False
```

This example demonstrates using the command to determine if a reboot is required to complete installation of
one or more Windows Updates.

### Example 2
```powershell
PS C:\> while ($true) {
    if (Test-WindowsUpdateRebootRequired) {
        Restart-Computer -Force
    }
    else {
        $Updates = @(Get-WindowsUpdate)
        if ($Updates.Count -eq 0) {
            break
        }
        else {
            $Updates | Request-WindowsUpdate -PassThru | Out-String | Write-Verbose
            $Updates | Install-WindowsUpdate -PassThru
        }
    }
}
```

This example uses a simple script to install Windows Updates until no further updates are required.
If after any update installation cycle a restart is required, the script will restart the computer.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Boolean

## NOTES

## RELATED LINKS
