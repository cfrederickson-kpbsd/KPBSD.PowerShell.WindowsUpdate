namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from c97ad11b-f257-420b-9d9f-377f733f6f68.
    /// </summary>
    [ModelForGuid("c97ad11b-f257-420b-9d9f-377f733f6f68")]
    public class UpdateDownloadContentModel : Model, IEquatable<UpdateDownloadContentModel>
    {
        private readonly dynamic _comObject;
        internal UpdateDownloadContentModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public string DownloadUrl { get { return _comObject.DownloadUrl; } }
		public bool IsDeltaCompressedContent { get { return _comObject.IsDeltaCompressedContent; } }

        public bool Equals(UpdateDownloadContentModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is UpdateDownloadContentModel && this.Equals((UpdateDownloadContentModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return DownloadUrl;
        }
    }
}
