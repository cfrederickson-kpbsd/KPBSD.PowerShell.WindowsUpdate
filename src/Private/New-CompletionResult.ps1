function New-CompletionResult {
    [System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Instantiates a .NET object. Does not change state.')]
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

        # Imitate the quotations of the IfLike string, if any, or use single quotes
        # if there is a space in the completion result text.
        [switch]
        $NoAutoQuoting
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
            if ($CompletionText -notlike "$IfLikeUnquoted*") {
                return
            }
        }
        else {
            $QuoteChar = $null
        }
        if (-not $NoAutoQuoting) {
            if (-not $QuoteChar -and ($CompletionText.Contains(' ') -or $CompletionText.Contains('$') -or $CompletionText.Contains('''') -or $CompletionText.Contains('"'))) {
                $QuoteChar = ''''
            }
            $CompletionText = "$QuoteChar$CompletionText$QuoteChar"
        }
        [System.Management.Automation.CompletionResult]::new(
            $CompletionText,
            $ListItemText,
            $ResultType,
            $ToolTip
        )
    }
}