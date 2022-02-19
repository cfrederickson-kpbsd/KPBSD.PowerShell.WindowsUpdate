function New-CompletionResult {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '')]
    [OutputType([System.Management.Automation.CompletionResult])]
    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline, Mandatory)]
        [string]
        $CompletionText,

        [Parameter(ValueFromPipeline)]
        [string]
        $ListItemText,

        [Alias('CompletionResultType')]
        [System.Management.Automation.CompletionResultType]
        $ResultType = 'ParameterValue',

        [Parameter(ValueFromPipeline)]
        [string]
        $ToolTip,

        # Only write the completion if it is similar to this wildcard pattern.
        [string]
        $IfLike,

        # Imitate the quotations of the IfLike string, if any.
        [switch]
        $AllowQuoting
    )
    process {
        if (-not $ListItemText) {
            $ListItemText = $CompletionText
        }
        if (-not $ToolTip) {
            $ToolTip = $CompletionText
        }
        if ($IfLike) {
            switch ($IfLike[0])
            {
                '''' { $QuoteChar = '''' }
                '"' { $QuoteChar = '"' }
                default { $QuoteChar = $null; $IfLikeUnquoted = $IfLike }
            }
            if ($QuoteChar) {
                if ($IfLike.EndsWith($QuoteChar)) {
                    $IfLikeUnquoted = $IfLike.Substring(1, $IfLike.Length - 2)
                }
                else {
                    $IfLikeUnquoted = $IfLike.Substring(1)
                }
            }
            if ($CompletionText -notlike $IfLikeUnquoted) {
                return
            }
            if ($AllowQuoting) {
                if (-not $QuoteChar -and $CompletionText.Contains(' ')) {
                    $QuoteChar = ''''
                }
                $CompletionText = "$QuoteChar$CompletionText$QuoteChar"
            }
        }
        [System.Management.Automation.CompletionResult]::new(
            $CompletionText,
            $ListItemText,
            $ResultType,
            $ToolTip
        )
    }
}