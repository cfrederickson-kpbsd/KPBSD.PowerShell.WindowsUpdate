namespace KPBSD.PowerShell.WindowsUpdate {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class ValidateEitherTypeNameAttribute : ValidateEnumeratedArgumentsAttribute {
        private readonly HashSet<string> _typeNames;
        public string[] TypeNames { get => this._typeNames.ToArray(); }
        public ValidateEitherTypeNameAttribute(params string[] typeNames) {
            this._typeNames = new HashSet<string>(typeNames, StringComparer.OrdinalIgnoreCase);
        }
        protected override void ValidateElement(object element)
        {
            var pso = PSObject.AsPSObject(element);
            foreach (var typename in pso.TypeNames)
            {
                if (this._typeNames.Contains(typename)) {
                    return;
                }
            }
            throw new ValidationMetadataException($"The value does not have any of the type names accepted by this parameter. Accepted type names: {string.Join("; ", this._typeNames)}");
        }
    }
}