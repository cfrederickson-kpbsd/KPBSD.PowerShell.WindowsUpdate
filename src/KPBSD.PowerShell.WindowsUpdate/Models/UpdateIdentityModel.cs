namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from 46297823-9940-4c09-aed9-cd3ea6d05968.
    /// </summary>
    [ModelForGuid("46297823-9940-4c09-aed9-cd3ea6d05968")]
    public class UpdateIdentityModel : Model, IEquatable<UpdateIdentityModel>
    {
        private readonly dynamic _comObject;
        public UpdateIdentityModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public int RevisionNumber { get { return _comObject.RevisionNumber; } }
		public string UpdateID { get { return _comObject.UpdateID; } }

        public bool Equals(UpdateIdentityModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is UpdateIdentityModel && this.Equals((UpdateIdentityModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} revision {1}", UpdateID, RevisionNumber);
        }
    }
}
