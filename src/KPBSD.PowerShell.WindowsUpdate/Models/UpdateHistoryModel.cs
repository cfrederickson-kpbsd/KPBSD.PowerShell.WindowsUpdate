namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from c2bfb780-4539-4132-ab8c-0a8772013ab6.
    /// </summary>
    [ModelForGuid("c2bfb780-4539-4132-ab8c-0a8772013ab6")]
    public class UpdateHistoryModel : Model, IEquatable<UpdateHistoryModel>
    {
        private readonly dynamic _comObject;
        public UpdateHistoryModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public string ClientApplicationID { get { return _comObject.ClientApplicationID; } }
		public DateTime Date { get { return _comObject.Date; } }
		public string Description { get { return _comObject.Description; } }
		public int HResult { get { return _comObject.HResult; } }
        public UpdateOperation Operation { get { return (UpdateOperation)(int)_comObject.Operation; } }
        public OperationResultCode ResultCode { get { return (OperationResultCode)(int)_comObject.ResultCode; } }
        public ServerSelection ServerSelection { get { return (ServerSelection)(int)_comObject.ServerSelection; } }
		public string ServiceID { get { return _comObject.ServiceID; } }
		public string SupportUrl { get { return _comObject.SupportUrl; } }
		public string Title { get { return _comObject.Title; } }
		public string UninstallationNotes { get { return _comObject.UninstallationNotes; } }
		public string[] UninstallationSteps { get { return ToStringArray(_comObject.UninstallationSteps); } }
		public int UnmappedResultCode { get { return _comObject.UnmappedResultCode; } }
		public UpdateIdentityModel UpdateIdentity { get { return new UpdateIdentityModel(_comObject.UpdateIdentity); } }

        public bool Equals(UpdateHistoryModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is UpdateHistoryModel && this.Equals((UpdateHistoryModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("'{0}' ({1} revision {2})",
                Title,
                UpdateIdentity.UpdateID,
                UpdateIdentity.RevisionNumber
                );
        }
    }
}
