using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class WUDownloadJob : WindowsUpdateJob
    {
        public WUDownloadJob(string command, string jobName)
            : base(command, jobName)
        {

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
            // Write job progress and output the completed download object if done.
            var currentDownload = downloadJob.Updates[callbackArgs.Progress.CurrentUpdateIndex];
            var currentDownloadComplete = callbackArgs.Progress.CurrentUpdatePercentComplete == 100;
            var downloadProgress = new ProgressRecord(
                (int)callbackArgs.Progress.CurrentUpdateIndex + 1,
                string.Format("Downloading Windows Update package {0}", callbackArgs.Progress.CurrentUpdateIndex),
                string.Format("Downloading '{0}' ({1} revision {2})", currentDownload.Title, currentDownload.Identity.UpdateId, currentDownload.Identity.Revision)
            )
            {
                CurrentOperation = string.Format("Downloading '{0}' ({1} revision {2})",
                    currentDownload.Title,
                    currentDownload.Identity.UpdateId,
                    currentDownload.Identity.Revision),
                RecordType = currentDownloadComplete ? ProgressRecordType.Completed : ProgressRecordType.Processing,
                PercentComplete = (int)callbackArgs.Progress.CurrentUpdatePercentComplete,
            };
            this.Progress.Add(downloadProgress);
            var totalComplete = callbackArgs.Progress.PercentComplete == 100;
            var totalProgress = new ProgressRecord(
                0,
                "Downloading Windows Update packages",
                string.Format("Downloading {0} Windows Update packages.", downloadJob.Updates.Count)
            )
            {
                RecordType = totalComplete ? ProgressRecordType.Completed : ProgressRecordType.Processing,
                PercentComplete = (int)callbackArgs.Progress.PercentComplete,
            };
            this.Progress.Add(totalProgress);

            if (currentDownloadComplete)
            {
                var downloadResult = callbackArgs.Progress.GetUpdateResult(callbackArgs.Progress.CurrentUpdateIndex);
                this.WriteDownloadResult(downloadResult.HResult, downloadResult.ResultCode, currentDownload);
            }
        }
        private void WriteDownloadResult(int hresult, OperationResultCode resultCode, object? update)
        {
            if (hresult != 0)
            {
                var exn = new COMException(null, hresult);
                var er = new ErrorRecord(
                    exn,
                    "IndividualDownloadError",
                    ErrorCategory.NotSpecified,
                    update
                );
                this.Error.Add(er);
            }
            switch (resultCode)
            {
                case OperationResultCode.Succeeded:
                    {
                        if (update != null)
                        {
                            this.Output.Add(PSObject.AsPSObject(update));
                        }
                    }
                    break;
            }
        }
        private void OnDownloadCompleted(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs)
        {
            var result = (IDownloadResult)this.WUJobSource!.GetType().InvokeMember(
                "EndDownload",
                BindingFlags.InvokeMethod,
                null,
                this.WUJobSource,
                new[] { downloadJob }
            );
            if (result.HResult != 0)
            {
                var exn = new COMException(null, result.HResult);
                var er = new ErrorRecord(
                    exn,
                    "DownloadError",
                    ErrorCategory.ReadError,
                    result
                );
                this.Error.Add(er);
            }
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