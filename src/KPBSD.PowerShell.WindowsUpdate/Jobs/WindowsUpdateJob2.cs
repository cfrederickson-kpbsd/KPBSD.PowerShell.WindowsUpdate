namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    public sealed class WindowsUpdateJob2 : Job
    {
        #region fields
        private bool _isSearchQueued;
        private bool _isSearchRunning;
        private bool _isSearchCompleted;
        private bool _isDownloadQueued;
        private bool _isDownloadRunning;
        private bool _isDownloadCompleted;
        private bool _isInstallQueued;
        private bool _isInstallRunning;
        private bool _isInstallCompleted;
        private bool _isUninstallation;
        /// <summary>
        /// Updates queued for the job prior to the job being started, such as the updates that will be
        /// downloaded by this job.
        /// </summary>
        private readonly List<IUpdate> _preQueuedUpdates;
        private dynamic _updateSession;
        private ISearchJob? _searchJob;
        private IDownloadJob? _downloadJob;
        private IInstallationJob? _installJob;
        #endregion

        #region properties

        #endregion

        #region abstract overrides
        /// <summary>
        /// Location at which the job is running. The computer name of the local host.
        /// </summary>
        /// <value></value>
        public override string Location { get { return Environment.MachineName; } }
        public override string StatusMessage { get { return string.Empty; } }
        public override bool HasMoreData
        {
            get
            {
                return this.Output.Count > 0
                || this.Error.Count > 0
                || this.Warning.Count > 0
                || this.Verbose.Count > 0
                || this.Debug.Count > 0
                || this.Information.Count > 0
                || this.Progress.Count > 0;
            }
        }
        #endregion

        #region private methods

        #endregion

        #region public methods
        public override void StopJob()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}