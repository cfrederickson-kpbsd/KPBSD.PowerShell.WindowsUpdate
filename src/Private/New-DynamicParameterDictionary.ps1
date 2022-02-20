function New-DynamicParameterDictionary {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType([System.Management.Automation.RuntimeDefinedParameterDictionary])]
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [System.Management.Automation.RuntimeDefinedParameter[]]
        $Parameter
    )
    begin {
        [System.Management.Automation.RuntimeDefinedParameterDictionary]$ParameterDictionary = [System.Management.Automation.RuntimeDefinedParameterDictionary]::new()
    }
    process {
        foreach ($p in $Parameter) {
            if ($ParameterDictionary.ContainsKey($P.Name)) {
                $ex = [System.ArgumentException]::new('The parameter has already been added to the dynamic parameter dictionary.')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'DynamicParameterNameConflict',
                    'InvalidArgument',
                    $P
                )
                $er.ErrorDetails = "The parameter '$($P.Name)' has already been added to the dynamic parameter dictionary."
                $er.ErrorDetails.RecommendedAction = 'Ensure that each parameter has a unique name.'
                $PSCmdlet.WriteError($er)
            }
            else {
                $ParameterDictionary.Add($P.Name, $P)
            }
        }
    }
    end {
        $ParameterDictionary
    }
}