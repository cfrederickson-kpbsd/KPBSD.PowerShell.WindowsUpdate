namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents one or more asynchronous Windows Update operations as a PowerShell job for seamless
    /// integration with PowerShell.
    /// </summary>
    public sealed class WindowsUpdateJob : Job
    {
        #region fields
        private volatile bool _isSearchQueued;
        private volatile bool _isDownloadQueued;
        private volatile bool _isInstallQueued;
        private int _maxCycleCount;
        private volatile int _currentCycleCount;
        private bool _isUninstallation;
        private CancellationTokenSource _cancellationTokenSource;
        private Task? _currentSearchTask;
        private Task? _currentInstallationTask;
        private Task? _currentDownloadTask;

        /// <summary>
        /// Updates queued for the "next" operation. This may be the first operation that the
        /// job completes, queued befores starting the job; or it may be updates identified by
        /// one step that will be passed to the next, for example if this job will both find
        /// and download updates.
        /// </summary>
        private readonly ConcurrentBag<IUpdate> _queuedUpdates;
        private readonly HashSet<long> _processedOutputIndex;
        private IUpdateSession? _updateSession;
        private SearchJobFilter? _searchFilter;
        #endregion

        #region properties
        private Task? CurrentTask { get { return (Task?)this._currentSearchTask ?? this._currentInstallationTask ?? this._currentInstallationTask; } }
        private CancellationToken CancellationToken { get { return _cancellationTokenSource.Token; } }
        private bool IsStopping { get { return _cancellationTokenSource.IsCancellationRequested; } }


        public bool IsSearching { get { return this._currentSearchTask != null; } }
        public bool IsDownloading { get { return this._currentDownloadTask != null; } }
        public bool IsInstalling { get { return !this._isUninstallation && this._currentInstallationTask != null; } }
        public bool IsUninstalling { get { return this._isUninstallation && this._currentInstallationTask != null; } }

        public bool IsSearchQueued { get { return this._isSearchQueued; } }
        public bool IsDownloadQueued { get { return this._isDownloadQueued; } }
        public bool IsInstallQueued { get { return this._isInstallQueued && !this._isUninstallation; } }
        public bool IsUninstallQueued { get { return this._isInstallQueued && this._isUninstallation; } }
        public SearchJobFilter? Filter { get { return this._searchFilter; } }
        public int SearchAndInstallCycleCount { get { return this._maxCycleCount; } }
        public int SearchAndInstallRemainingCycleCount { get { return this._currentCycleCount; } }
        #endregion

        #region abstract overrides
        /// <summary>
        /// Location at which the job is running. The computer name of the local host.
        /// </summary>
        /// <value></value>
        public override string Location { get { return Environment.MachineName; } }
        public override string StatusMessage { get { return "Task Status: " + (this.CurrentTask?.Status.ToString() ?? "Not Running"); } }
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
        /// <summary>
        /// Throws an exception if the job state does not equal <see cref="JobState.NotStarted"/>.
        /// </summary>
        private void AssertNotStarted()
        {
            if (this.JobStateInfo.State != JobState.NotStarted)
            {
                throw new InvalidJobStateException(this.JobStateInfo.State, "Jobs can only be started once.");
            }
        }
        /// <summary>
        /// Reports the exception as an error in the job streams, and sets the job state to
        /// <see cref="JobState.Failed"/>.
        /// </summary>
        /// <param name="exn"></param>
        private void FailWithException(Exception exn)
        {
            if (exn == null)
            {
                exn = new Exception("An unknown problem caused the job to fail.");
            }
            var er = new ErrorRecord(
                exn,
                "JobTerminatingException",
                ErrorCategory.NotSpecified,
                null
            );
            this.Error.Add(er);
            this.SetJobState(JobState.Failed);
        }
        private void WriteInformation<T>(T information, Func<T, string>? toString = null, params string[] tags)
        {
            InformationRecord ir;
            if (toString == null)
            {
                ir = new InformationRecord(information, this.Name);
            }
            else
            {
                var data = new StringFormattedInformation<T>(information, toString);
                ir = new InformationRecord(data, this.Name);
            }
            ir.Tags.AddRange(tags);
            this.Information.Add(new InformationRecord(information, this.Name));
        }
        /// <summary>
        /// Write a message to the job's debug stream.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        private void WriteDebug(string message, [CallerMemberName] string methodName = "")
        {
            var debugMessage = string.Format("{0} [{1}:{2}.{3}] {4}", DateTime.Now.ToString("HH:mm:ss.ffff"), Environment.CurrentManagedThreadId, GetType().Name, methodName, message);
            if (this.Debug.IsOpen)
            {
                this.Debug.Add(new DebugRecord(debugMessage));
            }
        }
        private void WriteProgress(int activityId, string activity, string status, string currentOperation, int percentComplete, int? parentActivityId)
        {
            var progress = new ProgressRecord(
                activityId,
                activity,
                status
            )
            {
                CurrentOperation = currentOperation,
                PercentComplete = percentComplete,
                RecordType = percentComplete == 100 ? ProgressRecordType.Completed : ProgressRecordType.Processing,
            };
            this.WriteDebug(string.Format("Writing progress {0}.", progress));
            if (parentActivityId.HasValue)
            {
                progress.ParentActivityId = parentActivityId.Value;
            }
            this.Progress.Add(progress);
            this.WriteDebug("WriteProgress completed.");
        }
        private void WriteDownloadProgress(IUpdate update, int index, int percentComplete, decimal bytesDownloaded, decimal totalBytes, DownloadPhase downloadPhase)
        {
            var activity = $"Downloading Windows Update {update.Title} ({update.Identity.UpdateID} revision {update.Identity.RevisionNumber})";
            var status = $"{downloadPhase} Windows Update ({bytesDownloaded} / {totalBytes} bytes downloaded).";
            var currentOperation = downloadPhase.ToString();
            int parentActivityId = (this.Id << 4) + this._currentCycleCount;
            int activityId = parentActivityId + 1 + index;
            this.WriteProgress(activityId, activity, status, currentOperation, percentComplete, parentActivityId);
        }
        private void WriteInstallationProgress(IUpdate update, int index, int percentComplete)
        {
            var un = this._isUninstallation ? "Uni" : "I";
            var activity = $"{un}nstalling {update.Title} ({update.Identity.UpdateID} revision {update.Identity.RevisionNumber})";
            var status = "Installing Windows Updates";
            int parentActivityId = (this.Id << 3) + this._currentCycleCount;
            int activityId = parentActivityId + 1 + index;
            this.WriteProgress(activityId, activity, status, activity, percentComplete, parentActivityId);
        }
        private void WriteDownloadTotalProgress(int percentComplete, decimal bytesDownloaded, decimal totalBytes)
        {
            var activity = "Downloading Windows Updates";
            var status = $"Downloading Windows Updates ({bytesDownloaded} / {totalBytes} bytes downloaded).";
            var activityId = (this.Id << 4) + this._currentCycleCount;
            this.WriteProgress(activityId, activity, status, activity, percentComplete, this.Id << 2);
        }
        private void WriteInstallationTotalProgress(int percentComplete)
        {
            var un = this._isUninstallation ? "Uni" : "I";
            var activity = $"{un}stalling Windows Updates";
            var activityId = (this.Id << 3) + this._currentCycleCount;
            this.WriteProgress(activityId, activity, "", "", percentComplete, this.Id << 2);
        }
        private void WriteTotalProgress(string currentOperation)
        {
            this.WriteProgress(this.Id << 2, "Running Windows Updates", currentOperation, currentOperation, 0, null);
        }
        /// <summary>
        /// Write an error record to the job's error stream if HResult indicates an unsuccessful state.
        /// If successful, the <paramref name="update"/> object will be written to the job's output
        /// stream.
        /// </summary>
        /// <param name="hresult"></param>
        /// <param name="resultCode"></param>
        /// <param name="update"></param>
        private void WriteOutputOrError(int hresult, OperationResultCode resultCode, IUpdate update)
        {
            if (hresult == 0)
            {
                try
                {
                    var model = Model.CreateModel(update);
                    var pso = PSObject.AsPSObject(model);
                    this.Output.Add(pso);
                }
                catch (ItemNotFoundException e)
                {
                    this.Warning.Add(new WarningRecord("Could not parse job output as WindowsUpdate model. Additional error data is included in the information stream. Error message: " + e.Message));
                    var infoRecord = new InformationRecord(e, this.Name);
                    infoRecord.Tags.Add("Error");
                    this.Information.Add(infoRecord);
                    this.Output.Add(PSObject.AsPSObject(update));
                }
                catch (FormatException e)
                {
                    var infoRecord = new InformationRecord(e, this.Name);
                    infoRecord.Tags.Add("Error");
                    this.Information.Add(infoRecord);
                    this.Output.Add(PSObject.AsPSObject(update));
                }
            }
            else
            {
                var er = ComErrorCodes.CreateErrorRecord(hresult, null, update);
                this.Error.Add(er);
            }
        }
        /// <summary>
        /// Must return bool because we are not throwing exception, just adding it to error stream.
        /// </summary>
        /// <returns></returns>
        private bool AssertCanBeginOperation()
        {
            if (this._updateSession == null)
            {
                this.FailWithException(new InvalidOperationException("Attempt to begin operation without providing a Windows Update session."));
                return false;
            }
            if (this._currentSearchTask != null || this._currentDownloadTask != null || this._currentInstallationTask != null)
            {
                this.FailWithException(new InvalidOperationException("Attempt to begin operation while another operation is running."));
                return false;
            }
            if (this.IsStopping)
            {
                this.WriteDebug("Attempt to begin operation while the job is stopping.");
                this.SetJobState(JobState.Stopped);
                return false;
            }
            return true;
        }
        private async Task BeginSearch()
        {
            try
            {
                if (!this.AssertCanBeginOperation())
                {
                    return;
                }
                this._processedOutputIndex.Clear();
                var updateSearcher = this._updateSession!.CreateUpdateSearcher();
                var searchTerms = new Collection<string>();
                if (this._searchFilter != null)
                {
                    updateSearcher.Online = !this._searchFilter.SearchOffline;
                    if (this._searchFilter.ServiceId != null)
                    {
                        updateSearcher.ServiceID = this._searchFilter.ServiceId;
                    }
                    updateSearcher.ServerSelection = this._searchFilter.ServerSelection;
                    updateSearcher.IncludePotentiallySupersededUpdates = this._searchFilter.IncludePotentiallySupersededUpdates;

                    if (this._searchFilter.Type.HasValue)
                    {
                        searchTerms.Add(string.Format("Type = '{0}'", this._searchFilter.Type));
                    }
                    if (this._searchFilter.IsSearchForUninstallations)
                    {
                        searchTerms.Add("DeploymentAction = 'Uninstallation'");
                    }
                    if (this._searchFilter.AssignedForAutomaticUpdates.HasValue)
                    {
                        searchTerms.Add(string.Format("IsAssigned = {0}", this._searchFilter.AssignedForAutomaticUpdates.Value ? 1 : 0));
                    }
                    if (this._searchFilter.BrowseOnly.HasValue)
                    {
                        searchTerms.Add(string.Format("BrowseOnly = {0}", this._searchFilter.BrowseOnly.Value ? 1 : 0));
                    }
                    if (this._searchFilter.AutoSelectOnWebSites.HasValue)
                    {
                        searchTerms.Add(string.Format("AutoSelectOnWebSites = {0}", this._searchFilter.AutoSelectOnWebSites.Value ? 1 : 0));
                    }
                    if (this._searchFilter.UpdateId?.Count == 1)
                    {
                        searchTerms.Add(string.Format("UpdateId = '{0}'", updateSearcher.EscapeString(this._searchFilter.UpdateId[0])));
                    }
                    if (this._searchFilter.CategoryId?.Count == 1)
                    {
                        searchTerms.Add(string.Format("CategoryIDs contains '{0}'", updateSearcher.EscapeString(this._searchFilter.CategoryId[0])));
                    }
                    if (!this._searchFilter.IncludeInstalled)
                    {
                        searchTerms.Add("IsInstalled = 0");
                    }
                    if (!this._searchFilter.IncludeHidden)
                    {
                        searchTerms.Add("IsHidden = 0");
                    }
                    if (this._searchFilter.IsPresent.HasValue)
                    {
                        searchTerms.Add(string.Format("IsPresent = {0}", this._searchFilter.IsPresent.Value ? 1 : 0));
                    }
                    if (this._searchFilter.RebootRequired.HasValue)
                    {
                        searchTerms.Add(string.Format("RebootRequired = {0}", this._searchFilter.RebootRequired.Value ? 1 : 0));
                    }

                    if (searchTerms.Count == 0 && (this._searchFilter.IncludeHidden || this._searchFilter.IncludeInstalled))
                    {
                        if (!this._searchFilter.IncludeHidden)
                        {
                            searchTerms.Add("IsHidden = 0");
                        }
                        else if (!this._searchFilter.IncludeInstalled)
                        {
                            searchTerms.Add("IsInstalled = 0");
                        }
                        else
                        {
                            // dummy so that the default `IsHidden = 0 and IsVisible = 0` is not used
                            // instead of my intentionally-empty string.
                            searchTerms.Add("IsHidden = 0 or IsHidden = 1");
                        }
                    }
                }
                var criteria = string.Join(" and ", searchTerms);
                this.WriteInformation(criteria);
                var searchResults = await updateSearcher.SearchAsync(criteria, this.CancellationToken);
                this._currentSearchTask = null;

                foreach (IUpdateException warning in searchResults.Warnings)
                {
                    var ex = new WindowsUpdateException(warning);
                    var er = ComErrorCodes.CreateErrorRecord((int)warning.HResult, ex, searchResults);
                    this.Error.Add(er);
                }
                var wildcardTitlesToMatch = new WildcardPattern[this._searchFilter?.Title?.Count ?? 0];
                for (int i = 0; i < wildcardTitlesToMatch.Length; i++)
                {
                    wildcardTitlesToMatch[i] = new WildcardPattern(this._searchFilter!.Title![i], WildcardOptions.IgnoreCase);
                }
                var updateIds = new HashSet<string>(
                    this._searchFilter?.UpdateId ?? Array.Empty<string>() as IEnumerable<string>,
                    StringComparer.OrdinalIgnoreCase
                    );
                foreach (IUpdate update in searchResults.Updates)
                {
                    // filter by everything possible before UpdateId and Title as those will be the slowest filters
                    if (this._searchFilter != null)
                    {
                        if (this._searchFilter.Type.HasValue && this._searchFilter.Type.Value != update.Type)
                        {
                            this.WriteDebug(string.Format("Filtered {0} by Type.", update.Title));
                            continue;
                        }
                        if (this._searchFilter.IsSearchForUninstallations && update.DeploymentAction != DeploymentAction.Uninstallation)
                        {
                            this.WriteDebug(string.Format("Filtered {0} by DeploymentAction (IsSearchForUninstallations).", update.Title));
                            continue;
                        }
                        // no client filter for AssignedForAutomaticUpdates
                        // no client filter for BrowseOnly
                        // no client filter for AutoSelectOnWebSites
                        // 
                        if (!this._searchFilter.IncludeInstalled && update.IsInstalled)
                        {
                            this.WriteDebug(string.Format("Filtered {0} by IsInstalled (IncludeInstalled).", update.Title));
                            continue;
                        }
                        if (!this._searchFilter.IncludeHidden && update.IsHidden)
                        {
                            this.WriteDebug(string.Format("Filtered {0} by IsHidden (IncludeHidden).", update.Title));
                            continue;
                        }
                        try
                        {
                            dynamic iupdate2 = update;
                            if (this._searchFilter.IsPresent.HasValue && this._searchFilter.IsPresent.Value != iupdate2.IsPresent)
                            {
                                this.WriteDebug(string.Format("Filtered {0} by IsPresent.", update.Title));
                                continue;
                            }
                            if (this._searchFilter.RebootRequired.HasValue && this._searchFilter.RebootRequired != iupdate2.RebootRequired)
                            {
                                this.WriteDebug(string.Format("Filtered {0} by RebootRequired.", update.Title));
                                continue;
                            }
                        }
                        catch
                        {
                            this.WriteDebug("Client filter RebootRequired and IsPresent could not be applied.");
                        }
                        try
                        {
                            dynamic iupdate3 = update;
                            if (this._searchFilter.BrowseOnly.HasValue && this._searchFilter.BrowseOnly != iupdate3.BrowseOnly)
                            {
                                this.WriteDebug(string.Format("Filtered {0} by BrowseOnly.", update.Title));
                                continue;
                            }
                        }
                        catch
                        {
                            this.WriteDebug("Client filter BrowseOnly could not be applied.");
                        }
                        if (updateIds.Count > 0 && !updateIds.Contains(update.Identity.UpdateID))
                        {
                            this.WriteDebug(string.Format("Filtered {0} by UpdateId.", update.Title));
                            continue;
                        }
                        var matchedTitle = wildcardTitlesToMatch.Length == 0;
                        foreach (var pattern in wildcardTitlesToMatch)
                        {
                            if (pattern.IsMatch(update.Title))
                            {
                                matchedTitle = true;
                                break;
                            }
                        }
                        if (!matchedTitle)
                        {
                            this.WriteDebug(string.Format("Filtered {0} by Title.", update.Title));
                            continue;
                        }
                    }

                    this._queuedUpdates.Add(update);
                }
                if (this._isDownloadQueued)
                {
                    this._currentDownloadTask = this.BeginDownload();
                }
                else if (this._isInstallQueued)
                {
                    this._currentInstallationTask = this.BeginInstall();
                }
                else
                {
                    while (this._queuedUpdates.TryTake(out var update))
                    {
                        var model = Model.CreateModel(update);
                        var pso = PSObject.AsPSObject(model);
                        this.Output.Add(pso);
                    }
                    this.SetFinishedStateByResultCode(searchResults.ResultCode);
                }
            }
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        private async Task BeginDownload()
        {
            try
            {
                if (!this.AssertCanBeginOperation())
                {
                    return;
                }
                if (this._queuedUpdates.Count == 0)
                {
                    this.WriteDebug("Attempt to begin download with no updates to process. Job completed.");
                    SetJobState(JobState.Completed);
                    return;
                }

                this._processedOutputIndex.Clear();
                var updateDownloader = this._updateSession!.CreateUpdateDownloader();
                updateDownloader.Updates = (IUpdateCollection)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
                // updateDownloader.Updates = (IUpdateCollection)new UpdateCollection();
                this.AddQueuedUpdatesToCollection(updateDownloader.Updates);

                var downloadResult = await updateDownloader.DownloadAsync(this.OnDownloadProgress, this.CancellationToken);
                this._currentDownloadTask = null;

                if (downloadResult.Result.HResult != 0)
                {
                    var comError = ComErrorCodes.CreateErrorRecord(downloadResult.Result.HResult, null, new { updateDownloader, downloadResult });
                    this.Error.Add(comError);
                }
                if (this._isInstallQueued)
                {
                    this._currentInstallationTask = this.BeginInstall();
                }
                else
                {
                    this.SetFinishedStateByResultCode(downloadResult.Result.ResultCode);
                }
            }
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        private async Task BeginInstall()
        {
            try
            {
                if (!this.AssertCanBeginOperation())
                {
                    return;
                }
                if (this._queuedUpdates.Count == 0)
                {
                    this.WriteDebug("Attempt to begin installation or uninstallation with no updates to process. Job completed.");
                    SetJobState(JobState.Completed);
                    return;
                }

                this._processedOutputIndex.Clear();
                var updateInstaller = this._updateSession!.CreateUpdateInstaller();
                updateInstaller.Updates = (IUpdateCollection)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
                // updateInstaller.Updates = (IUpdateCollection)new UpdateCollection();
                this.AddQueuedUpdatesToCollection(updateInstaller.Updates);

                JobResult<IInstallationResult> installationResult;
                if (this._isUninstallation)
                {
                    installationResult = await updateInstaller.UninstallAsync(this.OnInstallProgress, this.CancellationToken);
                }
                else
                {
                    installationResult = await updateInstaller.InstallAsync(this.OnInstallProgress, this.CancellationToken);
                }
                var result = installationResult.Result;
                if (result.RebootRequired)
                {
                    this.Warning.Add(
                        new WarningRecord(
                            "One or more updates installed on the computer require the computer to restart to continue."
                        )
                    );
                }
                if (result.HResult != 0)
                {
                    var comError = ComErrorCodes.CreateErrorRecord(result.HResult, null, new { updateInstaller, installationResult });
                    this.Error.Add(comError);
                }
                if (++this._currentCycleCount < this._maxCycleCount && this._isSearchQueued)
                {
                    this._currentSearchTask = this.BeginSearch();
                }
                else
                {
                    this.SetFinishedStateByResultCode(result.ResultCode);
                }
            }
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        private bool TryAddOutputIndex(long outputIndex)
        {
            this.WriteDebug(string.Format("Processing completion of index {0}", outputIndex));
            lock (_processedOutputIndex)
            {
                return this._processedOutputIndex.Add(outputIndex);
            }
        }
        private void SetFinishedStateByResultCode(OperationResultCode resultCode, [CallerMemberName] string callerMethod = "")
        {
            this.WriteInformation(resultCode, orc => string.Format("Setting job completion state from result code {0} while processing {1}.", orc, callerMethod));
            switch (resultCode)
            {
                case OperationResultCode.Aborted:
                    {
                        SetJobState(JobState.Stopped);
                    }
                    break;
                case OperationResultCode.Failed:
                    {
                        SetJobState(JobState.Failed);
                    }
                    break;
                case OperationResultCode.Succeeded:
                case OperationResultCode.SucceededWithErrors:
                    {
                        SetJobState(JobState.Completed);
                    }
                    break;
            }
        }
        private void AddQueuedUpdatesToCollection(IUpdateCollection updateCollection)
        {
            while (this._queuedUpdates.TryTake(out var update))
            {
                updateCollection.Add(update);
            }
        }
        private void OnDownloadProgress(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
        {
            this.WriteTotalProgress("Downloading");
            var currentUpdate = downloadJob.Updates[callbackArgs.Progress.CurrentUpdateIndex];
            this.WriteDownloadProgress(
                currentUpdate,
                (int)callbackArgs.Progress.CurrentUpdateIndex,
                (int)callbackArgs.Progress.CurrentUpdatePercentComplete,
                callbackArgs.Progress.CurrentUpdateBytesDownloaded,
                callbackArgs.Progress.CurrentUpdateBytesToDownload,
                callbackArgs.Progress.CurrentUpdateDownloadPhase
                );

            if (callbackArgs.Progress.CurrentUpdatePercentComplete == 100)
            {
                if (this.TryAddOutputIndex(callbackArgs.Progress.CurrentUpdateIndex))
                {
                    if (this._isInstallQueued)
                    {
                        this._queuedUpdates.Add(currentUpdate);
                    }
                    else
                    {
                        var model = Model.CreateModel(currentUpdate);
                        var pso = PSObject.AsPSObject(model);
                        this.Output.Add(pso);
                    }
                }
                else
                {
                    this.WriteDebug(
                        string.Format(
                            "The update {0} at index {1} was already enqueued processed.",
                            currentUpdate.Title,
                            callbackArgs.Progress.CurrentUpdateIndex
                            )
                        );
                }
            }

            this.WriteDownloadTotalProgress(
                (int)callbackArgs.Progress.PercentComplete,
                callbackArgs.Progress.TotalBytesDownloaded,
                callbackArgs.Progress.CurrentUpdateBytesToDownload
                );
        }
        private void OnInstallProgress(IInstallationJob installationJob, IInstallationProgressChangedCallbackArgs callbackArgs)
        {
            this.WriteTotalProgress("Installing");
            var currentUpdate = installationJob.Updates[callbackArgs.Progress.CurrentUpdateIndex];
            this.WriteInstallationProgress(currentUpdate, (int)callbackArgs.Progress.CurrentUpdateIndex, (int)callbackArgs.Progress.CurrentUpdatePercentComplete);
            if (callbackArgs.Progress.PercentComplete == 100)
            {
                if (this.TryAddOutputIndex(callbackArgs.Progress.CurrentUpdateIndex))
                {
                    if (this._currentCycleCount > 0)
                    {
                        this._queuedUpdates.Add(currentUpdate);
                    }
                    else
                    {
                        var model = Model.CreateModel(currentUpdate);
                        var pso = PSObject.AsPSObject(model);
                        this.Output.Add(pso);
                    }
                }
            }

            this.WriteInstallationTotalProgress((int)callbackArgs.Progress.PercentComplete);
        }
        #endregion

        #region public methods

        public WindowsUpdateJob(string? command, string? jobName) : base(command, jobName)
        {
            this.PSJobTypeName = nameof(WindowsUpdateJob);
            this._queuedUpdates = new ConcurrentBag<IUpdate>();
            this._processedOutputIndex = new HashSet<long>();
            this._cancellationTokenSource = new CancellationTokenSource();
        }
        public override void StopJob()
        {
            this.WriteDebug("Stopping job.");
            switch (this.JobStateInfo.State)
            {
                case JobState.Stopped:
                    {
                    }
                    break;
                case JobState.NotStarted:
                    {
                        this.SetJobState(JobState.Stopped);
                    }
                    break;
                case JobState.Stopping:
                    {
                        this.Finished.WaitOne();
                    }
                    break;
                case JobState.Running:
                    {
                        this.SetJobState(JobState.Stopping);
                        this._cancellationTokenSource.Cancel();
                        this.WriteDebug("Waiting for the job to finish.");
                        this.Finished.WaitOne();
                        break;
                    }
                default:
                    {
                        throw new InvalidJobStateException(this.JobStateInfo.State, "The job cannot be stopped unless the current JobState is Running.");
                    }
            }
        }
        internal void SetAsSearch(SearchJobFilter? filter)
        {
            this.AssertNotStarted();
            this._searchFilter = filter;
            this._isSearchQueued = true;
        }
        internal void SetAsDownload()
        {
            this.AssertNotStarted();
            this._isDownloadQueued = true;
        }
        internal void SetAsInstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = true;
            this._isUninstallation = false;
        }
        internal void SetAsUninstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = true;
            this._isUninstallation = true;
        }
        public void SetRepeatCycleCount(int repeatCycleCount)
        {
            this.AssertNotStarted();
            this._maxCycleCount = repeatCycleCount;
        }
        internal void StartJob(IUpdateSession updateSession)
        {
            this.AssertNotStarted();
            this._updateSession = updateSession;

            this.SetJobState(JobState.Running);
            if (this._isSearchQueued)
            {
                this._currentSearchTask = this.BeginSearch();
            }
            else if (this._isDownloadQueued)
            {
                this._currentDownloadTask = this.BeginDownload();
            }
            else if (this._isInstallQueued)
            {
                this._currentInstallationTask = this.BeginInstall();
            }
            else
            {
                this.SetJobState(JobState.Completed);
            }
        }
        internal void AddUpdate(UpdateModel update)
        {
            AssertNotStarted();
            this._queuedUpdates.Add((IUpdate)update.ComObject);
        }
        protected override void Dispose(bool disposing)
        {
            this._cancellationTokenSource.Cancel();
            if (disposing)
            {
                this._cancellationTokenSource.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Returns the name of the job.
        /// </summary>
        /// <returns>The name of the job.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        #region private classes
        public class StringFormattedInformation<T>
        {
            public static implicit operator T(StringFormattedInformation<T> wrapper) { return wrapper.Information; }
            public static implicit operator StringFormattedInformation<T>(T value) { return new StringFormattedInformation<T>(value, v => v?.ToString() ?? string.Empty); }
            public T Information { get; }
            private readonly Func<T, string> _toString;
            public override string ToString()
            {
                return this._toString(Information);
            }
            public StringFormattedInformation(T information, Func<T, string> toString)
            {
                this._toString = toString;
                Information = information;
            }
        }
        private sealed class Disposable : IDisposable
        {
            public static Disposable Indisposable { get; } = new Disposable();
            private Action? _onDisposed;
            private readonly object? _allowMultipleExecutions;
            private readonly bool _alwaysDispose;
            private Disposable()
            {

            }
            public Disposable(Action onDisposed, bool allowMutlipleExecutions, bool alwaysDispose)
            {
                if (onDisposed == null)
                {
                    throw new ArgumentNullException(nameof(onDisposed));
                }
                _onDisposed = onDisposed;
            }
            public Disposable(Action onDisposed)
                : this(onDisposed, false, false)
            {

            }
            ~Disposable()
            {
                if (_alwaysDispose)
                {
                    this.Dispose();
                }
            }
            public void Dispose ()
            {
                if (_onDisposed != null)
                {
                    if (_allowMultipleExecutions != null)
                    {
                        lock(_allowMultipleExecutions)
                        {
                            _onDisposed();
                            _onDisposed = null;
                        }
                    }
                    else
                    {
                        _onDisposed();
                        _onDisposed = null;
                    }
                }
            }
        }
        #endregion
    }
}