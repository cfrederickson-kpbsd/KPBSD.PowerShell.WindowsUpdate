namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    // I manually mapped this one and don't actually know the COM guid.
    // [ModelForGuid("$ComGuid")]
    public class WindowsDriverUpdateEntryModel : Model, IEquatable<WindowsDriverUpdateEntryModel>
    {
        private readonly dynamic _comObject;
        public WindowsDriverUpdateEntryModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public long DeviceProblemNumber { get { return _comObject.DeviceProblemNumber; } }
        public long DeviceStatus { get { return _comObject.DeviceStatus; } }
        public string DriverClass { get { return _comObject.DriverClass; } }
		public string DriverHardwareID { get { return _comObject.DriverHardwareID; } }
		public string DriverManufacturer { get { return _comObject.DriverManufacturer; } }
		public string DriverModel { get { return _comObject.DriverModel; } }
		public string DriverProvider { get { return _comObject.DriverProvider; } }
		public DateTime DriverVerDate { get { return _comObject.DriverVerDate; } }
		
        public bool Equals(WindowsDriverUpdateEntryModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is WindowsDriverUpdateEntryModel && this.Equals((UpdateDownloadContentModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Driver Update for {0} ({1})",
                DriverModel,
                DriverVerDate
                );
        }
    }
}
