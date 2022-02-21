
<#
.SYNOPSIS
Creates a C# class file that wraps a COM object.
#>
[CmdletBinding()]
param(
    # Name of the class.
    [Parameter(Mandatory)]
    [string]
    $TypeName,

    # Directory of path create the class file at.
    [Parameter(Mandatory)]
    [string]
    $OutputDirectory,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]
    $DerivedFrom = 'Model',

    # COM object to model a class after.
    [Parameter(Mandatory)]
    [psobject]
    $InputObject
)
process {
    if ($DerivedFrom -eq 'Model')
    {
        $BaseConstructor = ''
    }
    else
    {
        $BaseConstructor = ' : base (comObject)'
    }
    $MemberInfo = Get-Member -InputObject $InputObject -MemberType Property
    if ($MemberInfo[0].TypeName -match '^System.__ComObject#{(?<guid>.*)}$')
    {
        $ComGuid = $matches['guid']
    }
    $Members = $MemberInfo | ForEach-Object {
        $MemberName = $_.Name
        $GetDefinition = "get { return _comObject.$MemberName; }"
        if ($_.Definition -match '^(?<typeName>\w*)') {
            $MemberTypeName = $matches['typeName']
            switch -Regex -CaseSensitive ($MemberTypeName)
            {
                '^Variant$' { $MemberTypeName = 'object' ; break }
                '^Date$' { $MemberTypeName = 'DateTime' ; break }
                '^IStringCollection$' { $MemberTypeName = 'string[]' ; $GetDefinition = "get { return ToStringArray(_comObject.$MemberName); }" ; break }
                '^I(?<model>\w*)Collection$' {
                    $MemberTypeName = "$($matches['model'])Model[]" ;
                    $GetDefinition = "get { return ToModelArray(_comObject.$MemberName); }"
                    Write-Warning "$MemberName is enumerable but the results are not projected to the corresponding model definition."
                    break
                }
                '^I(?<model>[A-Z].*)$' {
                    $MemberTypeName = "$($matches['model'])Model"
                    $GetDefinition = "get { return ($MemberTypeName)Model.CreateModel(_comObject.$MemberName); }"
                    break
                }
            }
        }
        else {
            $MemberTypeName = 'object'
        }
        if ($_.Definition -like '*{set}*') {
            $SetDefinition = " set { _comObject.$MemberName = value.ComObject; }"
        }
        else {
            $SetDefinition = ''
        }
        "public $MemberTypeName $MemberName { $GetDefinition$SetDefinition }"
    }
@"
namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from $ComGuid.
    /// </summary>
    [ModelForGuid("$ComGuid")]
    public class $TypeName : $DerivedFrom, IEquatable<$TypeName>
    {
        private readonly dynamic _comObject;
        public $TypeName(object comObject)$BaseConstructor
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        $($Members -join "`n`t`t")

        public bool Equals($TypeName other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is $TypeName && this.Equals(($TypeName)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }
    }
}
"@ | Out-File -Path (Join-Path $OutputDirectory -ChildPath "$TypeName.cs")
}