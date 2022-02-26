using System;

namespace KPBSD.PowerShell.WindowsUpdate
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ModelForGuidAttribute : Attribute
    {
        public ModelForGuidAttribute(string guid)
        {
            Guid = Guid.Parse(guid);
        }
        public Guid Guid { get; }
    }
}