# Fails if the user is not running as admin.
# Optionally will just write a warning instead.
function Assert-RunAsAdmin {
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        # The operation that requires administrative privilages.
        [Parameter(Mandatory)]
        [string]
        $Operation,

        [Parameter()]
        [string[]]
        $Consequence,

        # Write a warning instead of throwing an error.
        [Parameter()]
        [switch]
        $AsWarning
    )
    process {
        if (Test-RunAsAdmin) {
            Write-Verbose "Performing the operation '$Operation', which requires administrative privilages."
            $true
        }
        else {
            if ($Consequence) {
                $Consequences = [String]::Join('; ', $Consequence)
                $ConsequenceMessage = " Proceeding may cause unintended consequences such as: $Consequences."
            }
            else {
                $ConsequenceMessage = $null
            }
            $MessageBase = "The operation '$Operation' requires administrative privilages."
            $Message = "$MessageBase$ConsequenceMessage"
            if ($AsWarning) {
                Write-Warning $Message
            }
            else {
                $ErrorMessage = $MessageBase
                $Exception = [System.UnauthorizedAccessException]::new($ErrorMessage)
                $ErrorRecord = [System.Management.Automation.ErrorRecord]::new(
                    $Exception,
                    'Unauthorized',
                    [System.Management.Automation.ErrorCategory]::PermissionDenied,
                    $null
                )
                $ErrorRecord.ErrorDetails = $Message
                $ErrorRecord.ErrorDetails.RecommendedAction = 'Launch PowerShell as an administrator and try again.'
                throw $ErrorRecord
                # $PSCmdlet.ThrowTerminatingError($ErrorRecord)
            }
            $false
        }
    }
}