using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class WUDownloadJob : WindowsUpdateJob
    {
        private readonly HashSet<long> _completed;
        public WUDownloadJob(string command, string jobName)
            : base(command, jobName)
        {
            _completed = new HashSet<long>();
        }
        protected override string Operation => "Download";
        protected override object?[] GetBeginJobParameters()
        {
            return new object?[]
            {
                // onProgress
                new OnDownloadProgressChangedCallback(this.OnProgressChanged),
                // onCompleted
                new OnDownloadCompletedCallback(this.OnDownloadCompleted),
                // state
                null
            };
        }
        private void OnProgressChanged(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
        {
            this.WriteDebug($"({callbackArgs.Progress.PercentComplete}%) Processing progress changed callback at index {callbackArgs.Progress.CurrentUpdateIndex} ({callbackArgs.Progress.CurrentUpdatePercentComplete}%) which is at phase {callbackArgs.Progress.CurrentUpdateDownloadPhase}.");
            // write progress for the individual download first (completes the progress bar if applicable)
            try {
            this.WriteProgress((int)callbackArgs.Progress.CurrentUpdateIndex, (int)callbackArgs.Progress.CurrentUpdatePercentComplete, callbackArgs.Progress.CurrentUpdateBytesDownloaded, callbackArgs.Progress.CurrentUpdateBytesToDownload, callbackArgs.Progress.CurrentUpdateDownloadPhase.ToString());
            }
            catch (Exception e) {
                this.Error.Add(new ErrorRecord(
                    e,
                    "Unknown",
                    ErrorCategory.NotSpecified,
                    callbackArgs
                ));
            }
            // if the download completed, write the downloaded update to the job output
            if (callbackArgs.Progress.CurrentUpdatePercentComplete >= 100)
            {
                var updateIndex = callbackArgs.Progress.CurrentUpdateIndex;
                if (_completed.Add(updateIndex))
                {
                    var currentDownload = downloadJob.Updates[updateIndex];
                    this.WriteDebug($"The current download {currentDownload.Title} has completed.");
                    var downloadResult = callbackArgs.Progress.GetUpdateResult(updateIndex);
                    this.WriteOutputOrError(downloadResult.HResult, downloadResult.ResultCode, currentDownload);
                    this.WriteDebug($"The current download {currentDownload.Title} was written to the job output.");   
                }
                else
                {
                    this.WriteDebug($"The current download at index {updateIndex} was already output from the job.");
                }
            }

            // write total progress to finish the total percent after updating child progress
            // so that we are not completing total progress before completing child progress
            this.WriteProgress(null, (int)callbackArgs.Progress.PercentComplete, callbackArgs.Progress.TotalBytesDownloaded, callbackArgs.Progress.TotalBytesToDownload, null);
        }
        private void WriteProgress(int? currentUpdateIndex, int percentComplete, decimal bytesDownloaded, decimal totalBytes, string? currentOperation)
        {
            int activityId;
            if (currentUpdateIndex.HasValue)
            {
                activityId = this.Id + 1 + currentUpdateIndex.Value;
            }
            else
            {
                activityId = this.Id;
            }
            var isComplete = percentComplete >= 100;
            string activity;
            string status;
            if (currentUpdateIndex.HasValue)
            {
                var currentDownloadingUpdate = this.WUApiJob!.Updates[currentUpdateIndex];
                activity = $"Downloading Windows Update {currentDownloadingUpdate.Title} ({currentDownloadingUpdate.Identity.UpdateID} revision {currentDownloadingUpdate.Identity.RevisionNumber})";
                status = $"Downloading Windows Update ({bytesDownloaded} / {totalBytes} bytes downloaded).";
                currentOperation ??= $"Downloading Windows Update {currentDownloadingUpdate.Title} ({currentDownloadingUpdate.Identity.UpdateID} revision {currentDownloadingUpdate.Identity.RevisionNumber})";
            }
            else
            {
                activity = $"Downloading Windows Updates";
                status = $"Downloading Windows Updates ({bytesDownloaded} / {totalBytes} bytes downloaded).";
            }
            var progress = new ProgressRecord(
                activityId,
                activity,
                status
            )
            {
                CurrentOperation = currentOperation,
                PercentComplete = percentComplete,
                RecordType = isComplete ? ProgressRecordType.Completed : ProgressRecordType.Processing,
            };
            if (currentUpdateIndex.HasValue)
            {
                progress.ParentActivityId = this.Id;
            }
            this.Progress.Add(progress);
        }
        private void WriteOutputOrError(int hresult, OperationResultCode resultCode, dynamic update)
        {
            this.WriteDebug($"Download for update {update.Title} completed with hresult {hresult} and result code {resultCode}.");
            if (hresult == 0)
            {
                this.Output.Add(PSObject.AsPSObject(Model.CreateModel(update)));
            }
            else
            {
                var exn = new COMException(null, hresult);
                var er = new ErrorRecord(
                    exn,
                    "DownloadError",
                    ErrorCategory.NotSpecified,
                    update
                );
                this.Error.Add(er);
            }
            this.WriteDebug("Completed writing download result.");
        }
        private void WriteDebug(string message, [CallerMemberName] string methodName = "")
        {
            this.Debug.Add(new DebugRecord($"{DateTime.Now:HH:mm:ss.ffff} [WUDownloadJob.{methodName}:{Environment.ProcessorCount}] {message}"));
        }
        private void OnDownloadCompleted(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
        {
            this.WriteDebug("Download completed.");
            var result = (IDownloadResult)this.WUJobSource!.GetType().InvokeMember(
                "EndDownload",
                BindingFlags.InvokeMethod,
                null,
                this.WUJobSource,
                new[] { downloadJob }
            );
            if (result.HResult != 0)
            {
                this.WriteDebug($"Download has exceptional HResult {result.HResult}.");
                var exn = new COMException(null, result.HResult);
                var er = new ErrorRecord(
                    exn,
                    "DownloadError",
                    ErrorCategory.ReadError,
                    result
                );
                this.Error.Add(er);
            }
            this.WriteDebug($"Download has result code {result.ResultCode}.");
            switch (result.ResultCode)
            {
                case OperationResultCode.Aborted:
                    {
                        this.SetJobState(JobState.Stopped);
                    }
                    break;
                case OperationResultCode.Failed:
                    {
                        this.SetJobState(JobState.Failed);
                    }
                    break;
                default:
                    {
                        this.SetJobState(JobState.Completed);
                    }
                    break;
            }
        }
        private sealed class OnDownloadProgressChangedCallback : IDownloadProgressChangedCallback
        {
            public OnDownloadProgressChangedCallback(Action<IDownloadJob, IDownloadProgressChangedCallbackArgs> onProgress)
            {
                if (onProgress == null)
                {
                    throw new ArgumentNullException(nameof(onProgress));
                }
                _action = onProgress;
            }
            private Action<IDownloadJob, IDownloadProgressChangedCallbackArgs> _action;
            public void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs)
            {
                _action(downloadJob, callbackArgs);
            }
        }
        private sealed class OnDownloadCompletedCallback : IDownloadCompletedCallback
        {
            public OnDownloadCompletedCallback(Action<IDownloadJob, IDownloadCompletedCallbackArgs> onCompleted)
            {
                if (onCompleted == null)
                {
                    throw new ArgumentNullException(nameof(onCompleted));
                }
                _action = onCompleted;
            }
            private readonly Action<IDownloadJob, IDownloadCompletedCallbackArgs> _action;
            public void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
            {
                _action(downloadJob, callbackArgs);
            }
        }
    }
}