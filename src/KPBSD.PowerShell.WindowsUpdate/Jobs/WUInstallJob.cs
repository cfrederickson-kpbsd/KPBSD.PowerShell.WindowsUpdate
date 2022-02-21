namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class WUInstallJob : WindowsUpdateJob
    {
        private readonly HashSet<long> _completed;
        public WUInstallJob(string command, string jobName) : base(command, jobName)
        {
            _completed = new HashSet<long>();
        }

        protected override string Operation => "Install";

        protected override object?[] GetBeginJobParameters()
        {
            return new object?[]
            {
                // onProgress
                new OnInstallationProgressChangedCallback(this.OnInstallationProgressChanged),
                // onCompleted
                new OnInstallationCompletedCallback(this.OnInstallationCompleted),
                // state
                null
            };
        }

        private void OnInstallationCompleted(IInstallationJob installationJob, IInstallationCompletedCallbackArgs args)
        {
            this.WriteDebug("Installation completed.");
            var result = (IInstallationResult)this.WUJobSource!.GetType().InvokeMember(
                "EndInstall",
                BindingFlags.InvokeMethod,
                null,
                this.WUJobSource,
                new[] { installationJob }
            );
            if (result.HResult != 0)
            {
                this.WriteDebug($"Installation has exceptional HResult {result.HResult}.");
                var exn = new COMException(null, result.HResult);
                var er = new ErrorRecord(
                    exn,
                    "InstallationError",
                    ErrorCategory.ReadError,
                    result
                );
                this.Error.Add(er);
            }
            this.WriteDebug($"Installation has result code {result.ResultCode}.");
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
        private void OnInstallationProgressChanged(IInstallationJob installationJob, IInstallationProgressChangedCallbackArgs callbackArgs)
        {
            this.WriteDebug($"({callbackArgs.Progress.PercentComplete}%) Processing progress changed callback at index {callbackArgs.Progress.CurrentUpdateIndex} ({callbackArgs.Progress.CurrentUpdatePercentComplete}%).");
            // write progress for the individual installation first (completes the progress bar if applicable)
            this.WriteProgress((int)callbackArgs.Progress.CurrentUpdateIndex, (int)callbackArgs.Progress.CurrentUpdatePercentComplete);
            // if the installation completed, write the installed update to the job output
            if (callbackArgs.Progress.CurrentUpdatePercentComplete >= 100)
            {
                var updateIndex = callbackArgs.Progress.CurrentUpdateIndex;
                if (_completed.Add(updateIndex))
                {
                    var currentInstallation = installationJob.Updates[updateIndex];
                    this.WriteDebug($"The current installation {currentInstallation.Title} has completed.");
                    var installationResult = callbackArgs.Progress.GetUpdateResult(updateIndex);
                    this.WriteOutputOrError(installationResult.HResult, installationResult.ResultCode, currentInstallation);
                    this.WriteDebug($"The current installation {currentInstallation.Title} was written to the job output.");   
                }
                else
                {
                    this.WriteDebug($"The current installation at index {updateIndex} was already output from the job.");
                }
            }

            // write total progress to finish the total percent after updating child progress
            // so that we are not completing total progress before completing child progress
            this.WriteProgress(null, (int)callbackArgs.Progress.PercentComplete);
        }

        private void WriteProgress(int? currentUpdateIndex, int percentComplete, string? currentOperation = null)
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
            if (currentUpdateIndex.HasValue)
            {
                var currentInstallingUpdate = this.WUApiJob!.Updates[currentUpdateIndex];
                activity = $"Installing Windows Update {currentInstallingUpdate.Title} ({currentInstallingUpdate.Identity.UpdateID} revision {currentInstallingUpdate.Identity.RevisionNumber})";
                currentOperation ??= $"Installing Windows Update {currentInstallingUpdate.Title} ({currentInstallingUpdate.Identity.UpdateID} revision {currentInstallingUpdate.Identity.RevisionNumber})";
            }
            else
            {
                activity = $"Installing Windows Updates";
            }
            var progress = new ProgressRecord(
                activityId,
                activity,
                "Installing Windows Updates"
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
        private sealed class OnInstallationCompletedCallback : IInstallationCompletedCallback
        {
            private readonly Action<IInstallationJob, IInstallationCompletedCallbackArgs> _action;

            public OnInstallationCompletedCallback(Action<IInstallationJob, IInstallationCompletedCallbackArgs> action)
            {
                _action = action;
            }
            public void Invoke(IInstallationJob job, IInstallationCompletedCallbackArgs callbackArgs)
            {
                _action(job, callbackArgs);
            }
        }
        private sealed class OnInstallationProgressChangedCallback : IInstallationProgressChangedCallback
        {
            private readonly Action<IInstallationJob, IInstallationProgressChangedCallbackArgs> _action;

            public OnInstallationProgressChangedCallback(Action<IInstallationJob, IInstallationProgressChangedCallbackArgs> action)
            {
                _action = action;
            }
            public void Invoke(IInstallationJob job, IInstallationProgressChangedCallbackArgs callbackArgs)
            {
                _action(job, callbackArgs);
            }
        }
    }
}