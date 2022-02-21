namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    public sealed class ModelTypeConverter : PSTypeConverter
    {
        public override bool CanConvertFrom(object sourceValue, Type destinationType)
        {
            try
            {
                return destinationType.IsAssignableFrom(Model.CreateModel(sourceValue).GetType());
            }
            catch
            {
                return false;
            }
        }

        public override bool CanConvertTo(object sourceValue, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            return Model.CreateModel(sourceValue);
        }

        public override object ConvertTo(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
        {
            throw new NotImplementedException();
        }
    }
}