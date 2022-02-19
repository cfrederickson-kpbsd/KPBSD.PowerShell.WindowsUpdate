using System;
using System.Management.Automation;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public class ServerSelectionArgumentValidationAttribute : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (LanguagePrimitives.TryConvertTo<ServerSelection>(arguments, out _)) {
                return;
            }
            if (LanguagePrimitives.TryConvertTo<Guid>(arguments, out _)) {
                return;
            }
            throw new ValidationMetadataException("The value must be a ServerSelection enumeration type or a guid representing the ServerId.");
        }
    }
}
