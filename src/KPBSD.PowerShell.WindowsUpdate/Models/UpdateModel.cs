namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from c1c2f21a-d2f4-4902-b5c6-8a081c19a890.
    /// </summary>
    [ModelForGuid("c1c2f21a-d2f4-4902-b5c6-8a081c19a890")]
    public class UpdateModel : Model, IEquatable<UpdateModel>
    {
        private readonly dynamic _comObject;
        public UpdateModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public AutoDownloadMode AutoDownload { get { return (AutoDownloadMode)(int)_comObject.AutoDownload; } }
		public AutoSelectionMode AutoSelection { get { return (AutoSelectionMode)(int)_comObject.AutoSelection; } }
		public bool AutoSelectOnWebSites { get { return _comObject.AutoSelectOnWebSites; } }
		public bool BrowseOnly { get { return _comObject.BrowseOnly; } }
		public UpdateModel[] BundledUpdates { get { return ToModelArray(_comObject.BundledUpdates); } }
		public bool CanRequireSource { get { return _comObject.CanRequireSource; } }
		public CategoryModel[] Categories { get { return ToModelArray(_comObject.Categories); } }
		public string[] CveIDs { get { return ToStringArray(_comObject.CveIDs); } }
		public object Deadline { get { return _comObject.Deadline; } }
		public bool DeltaCompressedContentAvailable { get { return _comObject.DeltaCompressedContentAvailable; } }
		public bool DeltaCompressedContentPreferred { get { return _comObject.DeltaCompressedContentPreferred; } }
		public DeploymentAction DeploymentAction { get { return (DeploymentAction)(int)_comObject.DeploymentAction; } }
		public string Description { get { return _comObject.Description; } }
		public UpdateDownloadContentModel[] DownloadContents { get { return ToModelArray(_comObject.DownloadContents); } }
		public DownloadPriority DownloadPriority { get { return (DownloadPriority)(int)_comObject.DownloadPriority; } }
		public bool EulaAccepted { get { return _comObject.EulaAccepted; } }
		public string EulaText { get { return _comObject.EulaText; } }
		public string HandlerID { get { return _comObject.HandlerID; } }
		public UpdateIdentityModel Identity { get { return new UpdateIdentityModel(_comObject.Identity); } }
		public ImageInformationModel Image { get { return new ImageInformationModel(_comObject.Image); } }
		public InstallationBehaviorModel InstallationBehavior { get { return new InstallationBehaviorModel(_comObject.InstallationBehavior); } }
		public bool IsBeta { get { return _comObject.IsBeta; } }
		public bool IsDownloaded { get { return _comObject.IsDownloaded; } }
		public bool IsHidden { get { return _comObject.IsHidden; } set { _comObject.IsHidden = value; } }
		public bool IsInstalled { get { return _comObject.IsInstalled; } }
		public bool IsMandatory { get { return _comObject.IsMandatory; } }
		public bool IsPresent { get { return _comObject.IsPresent; } }
		public bool IsUninstallable { get { return _comObject.IsUninstallable; } }
		public string[] KBArticleIDs { get { return ToStringArray(_comObject.KBArticleIDs); } }
		public string[] Languages { get { return ToStringArray(_comObject.Languages); } }
		public DateTime LastDeploymentChangeTime { get { return _comObject.LastDeploymentChangeTime; } }
		public decimal MaxDownloadSize { get { return _comObject.MaxDownloadSize; } }
		public decimal MinDownloadSize { get { return _comObject.MinDownloadSize; } }
		public string[] MoreInfoUrls { get { return ToStringArray(_comObject.MoreInfoUrls); } }
		public string MsrcSeverity { get { return _comObject.MsrcSeverity; } }
		public bool PerUser { get { return _comObject.PerUser; } }
		public bool RebootRequired { get { return _comObject.RebootRequired; } }
		public int RecommendedCpuSpeed { get { return _comObject.RecommendedCpuSpeed; } }
		public int RecommendedHardDiskSpace { get { return _comObject.RecommendedHardDiskSpace; } }
		public int RecommendedMemory { get { return _comObject.RecommendedMemory; } }
		public string ReleaseNotes { get { return _comObject.ReleaseNotes; } }
		public string[] SecurityBulletinIDs { get { return ToStringArray(_comObject.SecurityBulletinIDs); } }
		public string[] SupersededUpdateIDs { get { return ToStringArray(_comObject.SupersededUpdateIDs); } }
		public string SupportUrl { get { return _comObject.SupportUrl; } }
		public string Title { get { return _comObject.Title; } }
		public UpdateType Type { get { return (UpdateType)(int)_comObject.Type; } }
		public InstallationBehaviorModel UninstallationBehavior { get { return new InstallationBehaviorModel(_comObject.UninstallationBehavior); } }
		public string UninstallationNotes { get { return _comObject.UninstallationNotes; } }
		public string[] UninstallationSteps { get { return ToStringArray(_comObject.UninstallationSteps); } }

        public bool Equals(UpdateModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is UpdateModel && this.Equals((UpdateModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

		public override string ToString()
		{
			return string.Format("'{0}' ({1} revision {2})",
				Title,
				Identity.UpdateID,
				Identity.RevisionNumber
				);
		}
    }
}
