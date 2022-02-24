namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from d9a59339-e245-4dbd-9686-4d5763e39624.
    /// </summary>
    [ModelForGuid("d9a59339-e245-4dbd-9686-4d5763e39624")]
    public class InstallationBehaviorModel : Model, IEquatable<InstallationBehaviorModel>
    {
        private readonly dynamic _comObject;
        internal InstallationBehaviorModel(object comObject)
        {
            this._comObject = comObject;
        }
        [System.Management.Automation.Hidden]
        public object ComObject { get { return _comObject; } }
        public bool CanRequestUserInput { get { return _comObject.CanRequestUserInput; } }
		public InstallationImpact Impact { get { return (InstallationImpact)(int)_comObject.Impact; } }
		public InstallationRebootBehavior RebootBehavior { get { return (InstallationRebootBehavior)(int)_comObject.RebootBehavior; } }
		public bool RequiresNetworkConnectivity { get { return _comObject.RequiresNetworkConnectivity; } }

        public bool Equals(InstallationBehaviorModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is InstallationBehaviorModel && this.Equals((InstallationBehaviorModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[ CanRequestUserInput: {0} ] [ Impact: {1} ] [ RebootBehavior: {2} ] [ RequiresNetworkConnectivity: {3} ]",
                CanRequestUserInput,
                Impact,
                RebootBehavior,
                RequiresNetworkConnectivity);
        }
    }
}
