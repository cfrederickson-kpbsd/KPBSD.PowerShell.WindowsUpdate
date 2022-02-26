namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using System.Collections.ObjectModel;
    using System.Threading;

    public sealed class WindowsUpdateJob2 : Job
    {
        #region fields
        private volatile bool _isSearchQueued;
        private volatile bool _isSearchRunning;
        private volatile bool _isSearchCompleted;
        private volatile bool _isDownloadQueued;
        private volatile bool _isDownloadRunning;
        private volatile bool _isDownloadCompleted;
        private volatile bool _isInstallQueued;
        private volatile bool _isInstallRunning;
        private volatile bool _isInstallCompleted;
        private volatile bool _isStopping;
        private int _maxCycleCount;
        private volatile int _remainingCycleCount;
        private bool _isUninstallation;
        private string _statusMessage;
        private Action? _cancellation;
        /// <summary>
        /// Updates queued for the "next" operation. This may be the first operation that the
        /// job completes, queued befores starting the job; or it may be updates identified by
        /// one step that will be passed to the next, for example if this job will both find
        /// and download updates.
        /// </summary>
        private readonly ConcurrentBag<IUpdate> _queuedUpdates;
        private readonly HashSet<long> _processedOutputIndex;
        private readonly CountdownEvent _cleanupsCompleted;
        private IUpdateSession? _updateSession;
        private SearchJobFilter? _searchFilter;
        #endregion

        #region properties
        public bool IsSearchQueued { get { return this._isSearchQueued; } }
        public bool IsDownloadQueued { get { return this._isDownloadQueued; } }
        public bool IsInstallQueued { get { return this._isInstallQueued && !this._isUninstallation; } }
        public bool IsUninstallQueued { get { return this._isInstallQueued && this._isUninstallation; } }
        public SearchJobFilter? Filter { get { return this._searchFilter; } }
        public int SearchAndInstallCycleCount { get { return this._maxCycleCount; } }
        public int SearchAndInstallRemainingCycleCount { get { return this._remainingCycleCount; } }
        #endregion

        #region abstract overrides
        /// <summary>
        /// Location at which the job is running. The computer name of the local host.
        /// </summary>
        /// <value></value>
        public override string Location { get { return Environment.MachineName; } }
        public override string StatusMessage { get { return this._statusMessage; } }
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
            this.Debug.Add(new DebugRecord($"{DateTime.Now:HH:mm:ss.ffff} [{GetType().Name}.{methodName}:{Environment.ProcessorCount}] {message}"));
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
            if (parentActivityId.HasValue)
            {
                progress.ParentActivityId = parentActivityId.Value;
            }
            this.Progress.Add(progress);
        }
        private void WriteDownloadProgress(IUpdate update, int index, int percentComplete, decimal bytesDownloaded, decimal totalBytes, DownloadPhase downloadPhase)
        {
            var activity = $"Downloading Windows Update {update.Title} ({update.Identity.UpdateID} revision {update.Identity.RevisionNumber})";
            var status = $"{downloadPhase} Windows Update ({bytesDownloaded} / {totalBytes} bytes downloaded).";
            var currentOperation = downloadPhase.ToString();
            int parentActivityId = (this.Id << 4) + this._remainingCycleCount;
            int activityId = parentActivityId + 1 + index;
            this.WriteProgress(activityId, activity, status, currentOperation, percentComplete, parentActivityId);
        }
        private void WriteInstallationProgress(IUpdate update, int index, int percentComplete)
        {
            var un = this._isUninstallation ? "Uni" : "I";
            var activity = $"{un}nstalling {update.Title} ({update.Identity.UpdateID} revision {update.Identity.RevisionNumber})";
            var status = "Installing Windows Updates";
            int parentActivityId = (this.Id << 3) + this._remainingCycleCount;
            int activityId = parentActivityId + 1 + index;
            this.WriteProgress(activityId, activity, status, activity, percentComplete, parentActivityId);
        }
        private void WriteDownloadTotalProgress(int percentComplete, decimal bytesDownloaded, decimal totalBytes)
        {
            var activity = "Downloading Windows Updates";
            var status = $"Downloading Windows Updates ({bytesDownloaded} / {totalBytes} bytes downloaded).";
            var activityId = (this.Id << 4) + this._remainingCycleCount;
            this.WriteProgress(activityId, activity, status, activity, percentComplete, this.Id << 2);
        }
        private void WriteInstallationTotalProgress(int percentComplete)
        {
            var un = this._isUninstallation ? "Uni" : "I";
            var activity = $"{un}stalling Windows Updates";
            var activityId = (this.Id << 3) + this._remainingCycleCount;
            this.WriteProgress(activityId, activity, "", "", percentComplete, this.Id << 2);
        }
        private void WriteTotalProgress(string currentOperation)
        {
            this._statusMessage = currentOperation;
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
            if (this._isSearchRunning || this._isDownloadRunning || this._isInstallRunning)
            {
                this.FailWithException(new InvalidOperationException("Attempt to begin operation while another operation is running."));
                return false;
            }
            if (this._isStopping)
            {
                this.WriteDebug("Attempt to begin operation while the job is stopping.");
                this.SetJobState(JobState.Stopped);
                return false;
            }
            return true;
        }
        private void BeginSearch()
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
            lock (this._updateSession)
            {
                var searchJob = updateSearcher.BeginSearch(
                    criteria,
                    new ComDelegate<ISearchJob, ISearchCompletedCallbackArgs>(this.OnSearchCompleted),
                    updateSearcher
                    );
                this._cancellation = () =>
                {
                    // we have to call CleanUp() on a separate thread, but to dispose
                    // we want to wait for the cleanup to complete.
                    this._cleanupsCompleted.AddCount();
                    searchJob.RequestAbort();
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        searchJob.CleanUp();
                        this._cleanupsCompleted.Signal();
                    });
                };
            }
        }
        private void BeginDownload()
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
            this._isDownloadCompleted = false;
            this._isDownloadRunning = true;
            var updateDownloader = this._updateSession!.CreateUpdateDownloader();
            // this._updateDownloader.Updates = (IUpdateCollection)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
            updateDownloader.Updates = (IUpdateCollection)new UpdateCollection();
            this.AddQueuedUpdatesToCollection(updateDownloader.Updates);
            lock (this._updateSession)
            {
                var downloadJob = updateDownloader.BeginDownload(
                    new ComDelegate<IDownloadJob, IDownloadProgressChangedCallbackArgs>(this.OnDownloadProgress),
                    new ComDelegate<IDownloadJob, IDownloadCompletedCallbackArgs>(this.OnDownloadCompleted),
                    updateDownloader
                    );
                this._cancellation = () =>
                {
                    // we have to call CleanUp() on a separate thread, but to dispose
                    // we want to wait for the cleanup to complete.
                    this._cleanupsCompleted.AddCount();
                    downloadJob.RequestAbort();
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        downloadJob.CleanUp();
                        this._cleanupsCompleted.Signal();
                    });
                };
            }
        }
        private void BeginInstall()
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
            this._isInstallCompleted = false;
            this._isInstallRunning = true;
            var updateInstaller = this._updateSession!.CreateUpdateInstaller();
            updateInstaller.Updates = (IUpdateCollection)new UpdateCollection();
            this.AddQueuedUpdatesToCollection(updateInstaller.Updates);
            lock (this._updateSession)
            {
                if (this._isUninstallation)
                {
                    var installJob = updateInstaller.BeginUninstall(
                        new ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>(this.OnInstallProgress),
                        new ComDelegate<IInstallationJob, IInstallationCompletedCallbackArgs>(this.OnInstallCompleted),
                        updateInstaller
                        );

                    this._cancellation = () =>
                    {
                        // we have to call CleanUp() on a separate thread, but to dispose
                        // we want to wait for the cleanup to complete.
                        this._cleanupsCompleted.AddCount();
                        installJob.RequestAbort();
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            installJob.CleanUp();
                            this._cleanupsCompleted.Signal();
                        });
                    };
                }
                else
                {
                    var installJob = updateInstaller.BeginInstall(
                        new ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>(this.OnInstallProgress),
                        new ComDelegate<IInstallationJob, IInstallationCompletedCallbackArgs>(this.OnInstallCompleted),
                        updateInstaller
                        );

                    this._cancellation = () =>
                    {
                        // we have to call CleanUp() on a separate thread, but to dispose
                        // we want to wait for the cleanup to complete.
                        this._cleanupsCompleted.AddCount();
                        installJob.RequestAbort();
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            installJob.CleanUp();
                            this._cleanupsCompleted.Signal();
                        });
                    };
                }
            }
        }
        private void WaitForCleanup()
        {
            while (this._cleanupsCompleted.Wait(1000))
            {
                try
                {
                    this.WriteDebug("Waiting for job cleanup.");
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("{0} {1} : Waiting for job cleanup.", DateTime.Now.ToString("HH:mm:ss.ffff"), this.Name);
                }
            }
        }
        private bool TryAddOutputIndex(long outputIndex)
        {
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

        private void OnSearchCompleted(ISearchJob searchJob, ISearchCompletedCallbackArgs callbackArgs)
        {
            this.WriteTotalProgress("Searching");
            var updateSearcher = (IUpdateSearcher)searchJob.AsyncState;

            this._isSearchCompleted = true;
            this._isSearchRunning = false;

            this._cleanupsCompleted.AddCount();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                searchJob.CleanUp();
                this._cleanupsCompleted.Signal();
            });

            var searchResults = updateSearcher.EndSearch(searchJob);
            foreach (IUpdateException warning in searchResults.Warnings)
            {
                var ex = new WindowsUpdateException(warning);
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
                this.BeginDownload();
            }
            else if (this._isInstallQueued)
            {
                this.BeginInstall();
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
        private void OnDownloadCompleted(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
        {
            this.WriteTotalProgress("Downloading");
            var downloader = (IUpdateDownloader)downloadJob.AsyncState;
            var result = downloader.EndDownload(downloadJob);

            this._isDownloadCompleted = true;
            this._isDownloadRunning = false;

            this._cleanupsCompleted.AddCount();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                downloadJob.CleanUp();
                this._cleanupsCompleted.Signal();
            });

            if (result.HResult != 0)
            {
                var comError = ComErrorCodes.CreateErrorRecord(result.HResult, null, new { downloader, downloadJob, result });
                this.Error.Add(comError);
            }
            if (this._isInstallQueued)
            {
                this.BeginInstall();
            }
            else
            {
                this.SetFinishedStateByResultCode(result.ResultCode);
            }
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
                    if (this._remainingCycleCount > 0)
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
        private void OnInstallCompleted(IInstallationJob installationJob, IInstallationCompletedCallbackArgs callbackArgs)
        {
            this.WriteTotalProgress("Installing");
            var installer = (IUpdateInstaller)installationJob.AsyncState;
            var result = installer.EndInstall(installationJob);

            this._isInstallCompleted = true;
            this._isInstallRunning = false;

            this._cleanupsCompleted.AddCount();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                installationJob.CleanUp();
                this._cleanupsCompleted.Signal();
            });

            if (result.RebootRequired)
            {
                this.Warning.Add(
                    new WarningRecord(
                        "One or more updates installed on the computer require the computer to restart to continue."
                    )
                );
            }
            if (result.ResultCode != 0)
            {
                var comError = ComErrorCodes.CreateErrorRecord(result.HResult, null, new { installer, installationJob, result });
                this.Error.Add(comError);
            }
            if (this._remainingCycleCount-- > 0 && this._isSearchQueued)
            {
                this.BeginSearch();
            }
            else
            {
                this.SetFinishedStateByResultCode(result.ResultCode);
            }
        }
        #endregion

        #region public methods

        public WindowsUpdateJob2(string command, string jobName) : base(command, jobName)
        {
            this.PSJobTypeName = nameof(WindowsUpdateJob);
            this._queuedUpdates = new ConcurrentBag<IUpdate>();
            this._processedOutputIndex = new HashSet<long>();
            this._statusMessage = "Pending Start";
            this._cleanupsCompleted = new CountdownEvent(0);
            // this._cancellationDelegates = new ConcurrentBag<Action>();
        }
        public override void StopJob()
        {
            this._isStopping = true;
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
                        lock (this._updateSession!)
                        {
                            if (_cancellation != null)
                            {
                                this._cancellation();
                            }
                        }
                        this.Finished.WaitOne();
                        break;
                    }
                default:
                    {
                        throw new InvalidJobStateException(this.JobStateInfo.State, "The job cannot be stopped unless the current JobState is Running.");
                    }
            }
            this.WaitForCleanup();
        }
        public void SetAsSearch(SearchJobFilter? filter)
        {
            this.AssertNotStarted();
            this._searchFilter = filter;
            this._isSearchQueued = true;
        }
        public void ClearSearch()
        {
            this.AssertNotStarted();
            this._searchFilter = null;
            this._isSearchQueued = false;
        }
        public void SetAsDownload()
        {
            this.AssertNotStarted();
            this._isDownloadQueued = true;
        }
        public void ClearDownload()
        {
            this.AssertNotStarted();
            this._isDownloadQueued = false;
        }
        public void SetAsInstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = true;
            this._isUninstallation = false;
        }
        public void ClearInstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = false;
            this._isUninstallation = false;
        }
        public void SetAsUninstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = true;
            this._isUninstallation = true;
        }
        public void ClearUninstall()
        {
            this.AssertNotStarted();
            this._isInstallQueued = false;
            this._isUninstallation = false;
        }
        public void SetRepeatCycleCount(int repeatCycleCount)
        {
            this.AssertNotStarted();
            this._maxCycleCount = repeatCycleCount;
        }
        public void StartJob()
        {
            this.AssertNotStarted();
            this.SetJobState(JobState.Running);
            if (this._isSearchQueued)
            {
                this.BeginSearch();
            }
            else if (this._isDownloadQueued)
            {
                this.BeginDownload();
            }
            else if (this._isInstallQueued)
            {
                this.BeginInstall();
            }
            else
            {
                this.SetJobState(JobState.Completed);
            }
        }
        public void AddUpdate(UpdateModel update)
        {
            AssertNotStarted();
            this._queuedUpdates.Add((IUpdate)update.ComObject);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._cancellation != null)
                {
                    this._cancellation();
                }
                this.WaitForCleanup();
                this._cleanupsCompleted.Dispose();
                this._statusMessage = "Disposed";
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
        private class ComDelegate<T1, T2> :
            ISearchCompletedCallback,
            IDownloadCompletedCallback,
            IInstallationCompletedCallback,
            IDownloadProgressChangedCallback,
            IInstallationProgressChangedCallback
        {
            public static implicit operator ComDelegate<T1, T2>(Action<T1, T2> action)
            {
                return new ComDelegate<T1, T2>(action);
            }
            private readonly Action<T1, T2> _action;
            public ComDelegate(Action<T1, T2> action)
            {
                this._action = action;
            }
            private void InvokeWithArguments<T3, T4>(T3 first, T4 second)
            {
                if (first is T1 asT1 && second is T2 asT2)
                {
                    this._action.Invoke(asT1, asT2);
                }
            }

            public void Invoke(ISearchJob job, ISearchCompletedCallbackArgs callbackArgs)
            {
                this.InvokeWithArguments(job, callbackArgs);
            }

            public void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
            {
                this.InvokeWithArguments(downloadJob, callbackArgs);
            }

            public void Invoke(IInstallationJob job, IInstallationCompletedCallbackArgs callbackArgs)
            {
                this.InvokeWithArguments(job, callbackArgs);
            }

            public void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
            {
                this.InvokeWithArguments(downloadJob, callbackArgs);
            }

            public void Invoke(IInstallationJob job, IInstallationProgressChangedCallbackArgs callbackArgs)
            {
                this.InvokeWithArguments(job, callbackArgs);
            }
        }
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
        #endregion
    }
}