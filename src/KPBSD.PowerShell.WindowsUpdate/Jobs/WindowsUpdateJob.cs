namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Reflection;

    public abstract class WindowsUpdateJob : Job
    {
        public override string Location { get { return ""; } }
        /// <summary>
        /// COM object representing the async operation.
        /// </summary>
        /// <value></value>
        protected dynamic? WUApiJob { get; set; }
        /// <summary>
        /// COM object which is the source of the async operation.
        /// </summary>
        /// <value></value>
        protected object? WUJobSource { get; private set; }
        protected abstract string Operation { get; }
        public override string StatusMessage
        {
            get
            {
                if (this.WUJobSource != null && this.WUApiJob != null)
                {
                    if (this.WUApiJob?.IsCompleted ?? false) {
                        return "Incomplete";
                    }
                    else {
                        dynamic result = this.WUJobSource!.GetType().InvokeMember(
                            string.Format("End{0}", this.Operation),
                            BindingFlags.InvokeMethod,
                            null,
                            this.WUJobSource,
                            new[] { this.WUApiJob }
                        );
                        return ((OperationResultCode)(int)result.ResultCode).ToString();
                    }
                }
                else
                {
                    return "Uninitialized";
                }
            }
        }
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
        
        protected void AssertNotStarted()
        {
            if (this.JobStateInfo.State != JobState.NotStarted)
            {
                throw new InvalidJobStateException(this.JobStateInfo.State, "Jobs can only be started once.");
            }
        }
        public override void StopJob()
        {
            if (this.JobStateInfo.State == JobState.Stopped)
            {
                return;
            }
            else if (this.JobStateInfo.State == JobState.NotStarted)
            {
                this.SetJobState(JobState.Stopped);
            }
            else if (this.JobStateInfo.State == JobState.Running)
            {
                this.SetJobState(JobState.Stopping);
                this.WUApiJob?.RequestAbort();
            }
            else
            {
                throw new InvalidJobStateException(this.JobStateInfo.State, "The job cannot be stopped unless the current JobState is Running.");
            }
        }
        protected void FailWithException(Exception exn)
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
        public void StartJob(object jobSource) {
            AssertNotStarted();
            if (jobSource is null)
            {
                throw new ArgumentNullException(nameof(jobSource));
            }
            try
            {
                this.SetJobState(JobState.Running);
                this.WUJobSource = jobSource;
                this.WUApiJob = this.WUJobSource.GetType().InvokeMember(
                    string.Format("Begin{0}", this.Operation),
                    BindingFlags.InvokeMethod,
                    null,
                    this.WUJobSource,
                    this.GetBeginJobParameters()
                );
            }
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        protected abstract object[] GetBeginJobParameters();
        protected virtual void AssertCanStart() {}
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (this.JobStateInfo.State == JobState.Running)
                {
                    this.StopJob();
                }
                if (this.WUApiJob != null)
                {
                    this.WUApiJob.CleanUp();
                }
            }
            base.Dispose(disposing);
        }

        public WindowsUpdateJob(string command, string jobName) : base (command, jobName)
        {
            this.PSJobTypeName = nameof(WindowsUpdateJob);
        }
    }
}