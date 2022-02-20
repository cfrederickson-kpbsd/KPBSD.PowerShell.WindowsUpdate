using System;
using System.Threading;
using System.Management.Automation;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class WUSearchJob : WindowsUpdateJob
    {
        private bool? _canAutomaticallyUpgradeService;
        private bool? _includePotentiallySupercededUpdates;
        private bool? _online;
        private ServerSelection? _serverSelection;
        private Guid? _serviceId;
        private bool? _ignoreDownloadPriority;
        private SearchScope? _searchScope;
        private string _criteria;
        private WindowsUpdateSearchParameters _clientFilterParameters;

        public WUSearchJob(string command, string jobName) : base(command, jobName)
        {
            _criteria = String.Empty;
            _clientFilterParameters = new WindowsUpdateSearchParameters(Array.Empty<WildcardPattern>(), Array.Empty<string>(), default, default);
        }
        protected override string Operation => "Search";
        public bool? CanAutomaticallyUpgradeService
        {
            get { return _canAutomaticallyUpgradeService; }
            set
            {
                AssertNotStarted();
                _canAutomaticallyUpgradeService = value;
            }
        }
        public bool? IncludePotentiallySupersededUpdates
        {
            get { return _includePotentiallySupercededUpdates; }
            set
            {
                AssertNotStarted();
                _includePotentiallySupercededUpdates = value;
            }
        }
        public bool? Online
        {
            get { return _online; }
            set
            {
                AssertNotStarted();
                _online = value;
            }
        }
        public ServerSelection? ServerSelection
        {
            get { return _serverSelection; }
            set
            {
                AssertNotStarted();
                _serverSelection = value;
            }
        }
        public Guid? ServiceId
        {
            get { return _serviceId; }
            set
            {
                AssertNotStarted();
                _serviceId = value;
            }
        }
        public bool? IgnoreDownloadPriority
        {
            get { return _ignoreDownloadPriority; }
            set
            {
                AssertNotStarted();
                _ignoreDownloadPriority = value;
            }
        }
        public SearchScope? SearchScope
        {
            get { return _searchScope; }
            set
            {
                AssertNotStarted();
                _searchScope = value;
            }
        }
        public string Criteria
        {
            get { return _criteria; }
            set
            {
                AssertNotStarted();
                if (value == null)
                {
                    throw new ArgumentNullException("Criteria");
                }
                _criteria = value;
            }
        }
        public WindowsUpdateSearchParameters ClientFilterParameters
        {
            get { return _clientFilterParameters; }
            set
            {
                AssertNotStarted();
                if (value == null)
                {
                    throw new ArgumentNullException("ClientFilterParameters");
                }
                _clientFilterParameters = value;
            }
        }
        protected override void AssertCanStart()
        {
            if (this.ServerSelection.GetValueOrDefault() == WindowsUpdate.ServerSelection.Others && !this.ServiceId.HasValue)
            {
                throw new InvalidOperationException("The ServiceId must be set when ServerSelection is [ServerSelection]::Others.");
            }
        }
        protected override object[] GetBeginJobParameters()
        {
            dynamic windowsUpdateSearcher = this.WUJobSource!;
            if (this.CanAutomaticallyUpgradeService.HasValue)
            {
                windowsUpdateSearcher.CanAutomaticallyUpgradeService = this.CanAutomaticallyUpgradeService.Value;
            }
            else
            {
                this._canAutomaticallyUpgradeService = windowsUpdateSearcher.CanAutomaticallyUpgradeService;
            }
            if (this.IncludePotentiallySupersededUpdates.HasValue)
            {
                windowsUpdateSearcher.IncludePotentiallySupersededUpdates = this.IncludePotentiallySupersededUpdates.Value;
            }
            else
            {
                this._includePotentiallySupercededUpdates = windowsUpdateSearcher.IncludePotentiallySupersededUpdates;
            }
            if (this.Online.HasValue)
            {
                windowsUpdateSearcher.Online = this.Online.Value;
            }
            else
            {
                this._online = windowsUpdateSearcher.Online;
            }
            if (this.ServiceId.HasValue)
            {
                windowsUpdateSearcher.ServiceId = this.ServiceId.Value.ToString();
            }
            else
            {
                this._serviceId = Guid.Parse(windowsUpdateSearcher.ServiceId);
            }
            if (this.ServerSelection.HasValue)
            {
                windowsUpdateSearcher.ServerSelection = (int)this.ServerSelection.Value;
            }
            else
            {
                this._serverSelection = (ServerSelection)(int)windowsUpdateSearcher.ServerSelection;
            }
            if (this.IgnoreDownloadPriority.HasValue)
            {
                windowsUpdateSearcher.IgnoreDownloadPriority = this.IgnoreDownloadPriority.Value;
            }
            else
            {
                this._ignoreDownloadPriority = windowsUpdateSearcher.IgnoreDownloadPriority;
            }
            if (this.SearchScope.HasValue)
            {
                windowsUpdateSearcher.SearchScope = (int)this.SearchScope.Value;
            }
            else
            {
                this._searchScope = (SearchScope)(int)windowsUpdateSearcher.SearchScope;
            }

            return new object[]
            {
                this.Criteria,
                new OnSearchCompletedCallback(OnSearchCompleted),
                windowsUpdateSearcher
            };
        }
        private void OnSearchCompleted(ISearchJob searchJob, ISearchCompletedCallbackArgs callbackArgs)
        {
            if (!searchJob.IsCompleted)
            {
                this.SetJobState(JobState.Stopped);
            }
            try
            {
                dynamic windowsUpdateSearcher = searchJob.AsyncState;
                // ISearchResult
                // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/aa386077(v=vs.85)
                var searchResult = windowsUpdateSearcher.EndSearch(searchJob);

                foreach (var warning in searchResult.Warnings)
                {
                    this.WriteError(warning);
                }

                foreach (var result in searchResult.Updates)
                {
                    if (_clientFilterParameters.Filter(result))
                    {
                        this.Output.Add(PSObject.AsPSObject(result));
                    }
                }
                
                switch ((OperationResultCode)(int)searchResult.ResultCode)
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
            catch (Exception e)
            {
                this.FailWithException(e);
            }
        }
        private void WriteError(dynamic searchResultError)
        {
            var exn = new WindowsUpdateException(searchResultError);
            var er = new ErrorRecord(
                exn,
                exn.Context.ToString(),
                ErrorCategory.NotSpecified,
                searchResultError
            );
            this.Error.Add(er);
        }



        private sealed class OnSearchCompletedCallback : ISearchCompletedCallback
        {
            public OnSearchCompletedCallback(Action<ISearchJob, ISearchCompletedCallbackArgs> action)
            {
                _action = action;
            }
            private Action<ISearchJob, ISearchCompletedCallbackArgs> _action;
            public void Invoke(ISearchJob searchJob, ISearchCompletedCallbackArgs args)
            {
                _action(searchJob, args);
            }
        }
    }
}