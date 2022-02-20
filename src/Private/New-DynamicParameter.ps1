function New-DynamicParameter {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
    [OutputType([System.Management.Automation.RuntimeDefinedParameter])]
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]
        $Name,

        [Parameter()]
        [ValidateNotNull()]
        [Alias('Type')]
        [string]
        $ParameterType = [psobject],

        [Parameter()]
        [Alias('Value')]
        [psobject]
        $DefaultValue,

        [Parameter()]
        [switch]
        $Mandatory,

        [Parameter()]
        [int]
        $Position = [int]::MinValue,

        [Parameter()]
        [switch]
        $DontShow,

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string]
        $HelpMessage,

        [Parameter()]
        [string]
        $ParameterSetName = [Parameter]::AllParameterSets,

        [Parameter()]
        [switch]
        $ValueFromPipeline,

        [Parameter()]
        [switch]
        $ValueFromPipelineByPropertyName,

        [Parameter()]
        [switch]
        $ValueFromRemainingArguments,

        # Aliases for the parameter.
        [Parameter()]
        [string[]]
        $Alias,

        # Constructed ArgumentCompleter attribute, type that implements IArgumentCompleter, or a script block
        # to offer completions for the argument.
        [Parameter()]
        [psobject]
        $ArgumentCompleter,

        # Argument validation attribute or a script to validate arguments.
        [Parameter()]
        [psobject[]]
        $ArgumentValidation,

        [Parameter()]
        [Attribute[]]
        $Attributes

    )
    process {
        if ($ParameterType -as [Type]) {
            [type]$DynamicParameterType = $ParameterType
            $pstypename = $null
        }
        else {
            [type]$DynamicParameterType = [psobject]
            $pstypename = [PSTypeNameAttribute]::new($ParameterType)
        }
        $ParameterAttribute = [Parameter]::new()
        $IncludeParameter = $false
        if ($PSBoundParameters.ContainsKey('Mandatory')) { $ParameterAttribute.Mandatory = $Mandatory ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('Position')) { $ParameterAttribute.Position = $Position ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('DontShow')) { $ParameterAttribute.DontShow = $DontShow ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('ParameterSetName')) { $ParameterAttribute.ParameterSetName = $ParameterSetName ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('HelpMessage')) { $ParameterAttribute.HelpMessage = $HelpMessage ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('ValueFromPipeline')) { $ParameterAttribute.ValueFromPipeline = $ValueFromPipeline ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('ValueFromPipelineByPropertyName')) { $ParameterAttribute.ValueFromPipelineByPropertyName = $ValueFromPipelineByPropertyName ; $IncludeParameter = $true }
        if ($PSBoundParameters.ContainsKey('ValueFromRemainingArguments')) { $ParameterAttribute.ValueFromRemainingArguments = $ValueFromRemainingArguments ; $IncludeParameter = $true }
        [Attribute[]]$DynamicParameterAttributes = @(
            $pstypename;
            if ($IncludeParameter) {
                $ParameterAttribute
            }
            if ($ArgumentCompleter -is [ScriptBlock]) {
                [ArgumentCompleter]::new($ArgumentCompleter)
            }
            elseif ($ArgumentCompleter -is [ArgumentCompleter]) {
                $ArgumentCompleter
            }
            elseif ($ArgumentCompleter -as [Type]) {
                [Type]$ArgumentCompleterType = $ArgumentCompleter
                [ArgumentCompleter]::new($ArgumentCompleterType)
            }
            elseif ($ArgumentCompleter) {
                $ex = [ArgumentException]::new('The value passed to the ArgumentCompleter parameter is invalid. The value must be a ScriptBlock, a type that implements IArgumentCompleter, or a constructed ArgumentCompleter attribute.', 'ArgumentCompleter')
                $er = [System.Management.Automation.ErrorRecord]::new(
                    $ex,
                    'ArgumentCompleterInvaild',
                    'InvalidArgument',
                    $ArgumentCompleter
                )
                $PSCmdlet.WriteError($er)
            }
            foreach ($Validator in $ArgumentValidation) {
                if ($Validator -is [ScriptBlock]) {
                    [ValidateScript]::new($Validator)
                }
                elseif ($Validator -is [System.Management.Automation.ValidateArgumentsAttribute]) {
                    $Validator
                }
                elseif ($Validator -as [Type]) {
                    try {
                        [Type]$ValidatorType = $Validator
                        if ($ValidatorType.IsAssignableTo([System.Management.Automation.ValidateArgumentsAttribute])) {
                            [Activator]::CreateInstance([Type]$Validator, [object[]]@())
                        }
                        else {
                            $ex = [ArgumentException]::new('The type is not derived from ValidateArgumentsAttribute.')
                            $er = [System.Management.Automation.ErrorRecord]::new(
                                $ex,
                                'ArgumentValiationInvalid',
                                'InvalidArgument',
                                $Validator
                            )
                            $er.ErrorDetails = "The type of the argument '$Validator' for the ArgumentValidation parameter is not derived from [ValidateArgumentsAttribute]."
                            $PSCmdlet.WriteError($er)
                        }
                    }
                    catch [System.Management.Automation.MethodInvocationException] {
                        $PSCmdlet.WriteError($_.ErrorRecord)
                    }
                }
                else {
                    $ex = [ArgumentException]::new('The value passed to the ArgumentValidation parameter is invalid. The value must be a ScriptBlock, a type that is derived from [ValidateArgumentsAttribute] and has a parameterless constructor, or an instance of a type that is derived from [ValidateArgumentsAttribute].')
                    $er = [System.Management.Automation.ErrorRecord]::new(
                        $ex,
                        'ArgumentValidationInvalid',
                        'InvalidArgument',
                        $Validator
                    )
                    $PSCmdlet.WriteError($er)
                }
            }
            if ($Alias) {
                [Alias]::new($Alias)
            }
            $Attributes
        ) | Where-Object { $null -ne $_ }
        $DynamicParameter = [System.Management.Automation.RuntimeDefinedParameter]::new(
            $Name,
            $DynamicParameterType,
            $DynamicParameterAttributes
        )
        $DynamicParameter.Value = $DefaultValue
        $DynamicParameter
    }
}