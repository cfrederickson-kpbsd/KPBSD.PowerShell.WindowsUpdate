namespace KPBSD.PowerShell.WindowsUpdate.Commands
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// The idea behind this cmdlet is that it will be used to start the jobs, and the StartJob
    /// method will be made internal. However, Add-Type needs to be able to import the assemblies
    /// as a module if we're going to do that.
    /// </summary>

    [Cmdlet(VerbsLifecycle.Start, "WindowsUpdateJob", DefaultParameterSetName = "Installation")]
    [OutputType(typeof(WindowsUpdateJob))]
    public sealed class StartWindowsUpdateJobCommand : PSCmdlet
    {
        private IUpdateSession? _updateSession;

        [Parameter(Mandatory = true)]
        [PSTypeName("System.__ComObject#{918efd1e-b5d8-4c90-8540-aeb9bdc56f9d}")]
        public object? WindowsUpdateSession
        {
            get { return _updateSession; }
            set { _updateSession = value as IUpdateSession ?? (value as PSObject)?.BaseObject as IUpdateSession; }
        }

        /// <summary>
        /// Windows Updates to be set in the job for download or installation.
        /// </summary>
        /// <value></value>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public UpdateModel[] WindowsUpdate { get; set; } = Array.Empty<UpdateModel>();

        /// <summary>
        /// A filter for the job if it is a search job.
        /// </summary>
        /// <value></value>
        [Parameter]
        [ValidateNotNull]
        public SearchJobFilter? Filter { get; set; }

        /// <summary>
        /// Indicates that the job should download the identified updates.
        /// </summary>
        /// <value></value>
        [Parameter]
        public SwitchParameter Download { get; set; }
        /// <summary>
        /// Indicates that the job should install the identified updates.
        /// </summary>
        /// <value></value>
        [Parameter(ParameterSetName = "Installation")]
        public SwitchParameter Install { get; set; }

        /// <summary>
        /// Indicates that the job should uninstall the identified updates.
        /// </summary>
        /// <value></value>
        [Parameter(ParameterSetName = "Uninstallation")]
        public SwitchParameter Uninstall { get; set; }

        /// <summary>
        /// The name of the Windows Update job.
        /// </summary>
        /// <value></value>
        [Parameter]
        public string? JobName { get; set; }
        /// <summary>
        /// The script to be executed by the job.
        /// </summary>
        /// <value></value>
        [Parameter]
        public string? Command { get; set; }

        protected override void ProcessRecord()
        {
            if (this._updateSession == null)
            {
                var ex = new ArgumentNullException(nameof(this.WindowsUpdateSession));
                var er = new ErrorRecord(ex, "WindowsUpdateSessionRequired", ErrorCategory.InvalidArgument, null);
                er.ErrorDetails = new ErrorDetails("The Windows Update Session parameter was not of the type expected and was interpreted as null.");
                this.ThrowTerminatingError(er);
                // Basically, this line is for the compiler's peace of mind and should never be reached.
                throw ex;
            }

            var job = new WindowsUpdateJob(this.Command, this.JobName);
            foreach (var update in this.WindowsUpdate)
            {
                job.AddUpdate(update);
            }
            if (this.Filter != null)
            {
                job.SetAsSearch(this.Filter);
            }
            if (this.Download)
            {
                job.SetAsDownload();
            }
            if (this.Install)
            {
                job.SetAsInstall();
            }
            if (this.Uninstall)
            {
                job.SetAsUninstall();
            }
            this.JobRepository.Add(job);
            job.StartJob(this._updateSession);
            this.WriteObject(job);
        }
    }
}