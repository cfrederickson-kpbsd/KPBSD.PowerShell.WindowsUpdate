# Module import script. This contains PowerShell source code for the module outside of any function
# definitions: for example, registering argument completers or setting up module state.

# The problem I run into with completions for this module is that the actual Get-WindowsUpdate
# command (through no fault of my own) is slow, and there doesn't seem to be a way to make it
# quickly check some cached set of updates.

# Parameters that I could offer timely completions for:
# Get-WindowsUpdate -Server (ServerSelection or ServiceID)
# Get-WindowsUpdateHistory -Title -UpdateId