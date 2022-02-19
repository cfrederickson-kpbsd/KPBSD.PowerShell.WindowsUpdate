namespace KPBSD.PowerShell.WindowsUpdate.Commands
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// The idea behind this cmdlet is that it will be used to start the jobs, and the StartJob
    /// method will be made internal. However, Add-Type needs to be able to import the assemblies
    /// as a module if we're going to do that.
    /// </summary>
    
    [Cmdlet(VerbsLifecycle.Start, "WindowsUpdateJob")]
    public sealed class StartWindowsUpdateJobCommand : PSCmdlet
    {
        /// <summary>
        /// The job to be started.
        /// </summary>
        /// <value></value>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public WindowsUpdateJob Job { get; set; } = null!;

        /// <summary>
        /// COM object used to initialize and run the job. Should be unique to the job.
        /// </summary>
        /// <value></value>
        [Parameter(Mandatory = true, Position = 0)]
        public object JobSource { get; set; } = null!;

        protected override void ProcessRecord()
        {
            this.JobRepository.Add(Job);
            Job.StartJob(JobSource);
            this.WriteObject(Job);
        }
    }
}