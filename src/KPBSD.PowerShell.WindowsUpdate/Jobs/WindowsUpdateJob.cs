namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

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
                        dynamic result = this.WUJobSource!.GetType().InvokeMember(
                            string.Format("End{0}", this.Operation),
                            BindingFlags.InvokeMethod,
                            null,
                            this.WUJobSource,
                            new[] { this.WUApiJob }
                        );
                        return ((OperationResultCode)(int)result.ResultCode).ToString();
                    }
                    else {
                        return "Incomplete";
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
        
        public WindowsUpdateJob(string command, string jobName) : base (command, jobName)
        {
            this.PSJobTypeName = nameof(WindowsUpdateJob);
        }
        /// <summary>
        /// Throws an exception if the job state does not equal <see cref="JobState.NotStarted"/>.
        /// </summary>
        protected void AssertNotStarted()
        {
            if (this.JobStateInfo.State != JobState.NotStarted)
            {
                throw new InvalidJobStateException(this.JobStateInfo.State, "Jobs can only be started once.");
            }
        }
        /// <summary>
        /// Terminates the job operation.
        /// </summary>
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
            this.Finished.WaitOne();
        }
        /// <summary>
        /// Reports the exception as an error in the job streams, and sets the job state to
        /// <see cref="JobState.Failed"/>.
        /// </summary>
        /// <param name="exn"></param>
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
        /// <summary>
        /// Initializes and begins the job.
        /// </summary>
        /// <param name="jobSource"></param>
        internal void StartJob(object jobSource) {
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
            catch (COMException e)
            {
                ErrorRecord error;
                if (ComErrorCodes.TryGetErrorDetails(e.ErrorCode, out _, out _, out _))
                {
                    error = ComErrorCodes.CreateErrorRecord(e.ErrorCode, e, null);
                    this.Error.Add(error);
                    this.SetJobState(JobState.Failed);
                }
                else
                {
                    this.FailWithException(e);
                }
            }
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        /// <summary>
        /// Gets parameters to pass to the COM object's Begin{Operation} method.
        /// </summary>
        /// <returns></returns>
        protected abstract object?[] GetBeginJobParameters();
        /// <summary>
        /// Throw an exception if the job cannot start. Called during <see cref="StartJob"/>.
        /// </summary>
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
                    ThreadPool.QueueUserWorkItem(CleanUpAsThreadPoolItem, (object)this.WUApiJob);
                }
            }
            base.Dispose(disposing);
        }
        private static void CleanUpAsThreadPoolItem(dynamic state)
        {
            state.CleanUp();
        }

        public override string ToString()
        {
            return this.Name;
        }

        
        protected void WriteDebug(string message, [CallerMemberName] string methodName = "")
        {
            this.Debug.Add(new DebugRecord($"{DateTime.Now:HH:mm:ss.ffff} [{GetType().Name}.{methodName}:{Environment.ProcessorCount}] {message}"));
        }
        
        protected void WriteOutputOrError(int hresult, OperationResultCode resultCode, object update)
        {
            if (hresult == 0)
            {
                try {
                    var model = Model.CreateModel(update);
                    var pso = PSObject.AsPSObject(model);
                    this.Output.Add(pso);
                }
                catch (ItemNotFoundException e)
                {
                    this.Warning.Add(new WarningRecord("Could not parse job output as WindowsUpdate model. Additional data is included in the error stream. Error message: " + e.Message));
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
                // var exn = new COMException(null, hresult);
                // var er = new ErrorRecord(
                //     exn,
                //     "DownloadError",
                //     ErrorCategory.NotSpecified,
                //     update
                // );
                this.Error.Add(er);
            }
        }
    }
}