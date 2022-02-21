namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from 70cf5c82-8642-42bb-9dbc-0cfd263c6c4f.
    /// </summary>
    [ModelForGuid("70cf5c82-8642-42bb-9dbc-0cfd263c6c4f")]
    public class WindowsDriverUpdateModel : UpdateModel, IEquatable<WindowsDriverUpdateModel>
    {
        private readonly dynamic _comObject;
        internal WindowsDriverUpdateModel(object comObject) : base (comObject)
        {
            this._comObject = comObject;
        }
		public int DeviceProblemNumber { get { return _comObject.DeviceProblemNumber; } }
		public int DeviceStatus { get { return _comObject.DeviceStatus; } }
		public string DriverClass { get { return _comObject.DriverClass; } }
		public string DriverHardwareID { get { return _comObject.DriverHardwareID; } }
		public string DriverManufacturer { get { return _comObject.DriverManufacturer; } }
		public string DriverModel { get { return _comObject.DriverModel; } }
		public string DriverProvider { get { return _comObject.DriverProvider; } }
		public DateTime DriverVerDate { get { return _comObject.DriverVerDate; } }
		public WindowsDriverUpdateEntryModel[] WindowsDriverUpdateEntries { get { return ToModelArray(_comObject.WindowsDriverUpdateEntries); } }

        public bool Equals(WindowsDriverUpdateModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is WindowsDriverUpdateModel && this.Equals((WindowsDriverUpdateModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }
    }
}
