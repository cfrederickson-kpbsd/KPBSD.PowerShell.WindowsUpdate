using System;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public sealed class AutomaticUpdateLastResults
    {
        public AutomaticUpdateLastResults(bool serviceEnabled, DateTime? searchUtc, DateTime? installationUtc)
        {
            IsServiceEnabled = serviceEnabled;
            LastSearchSuccessDateUtc = searchUtc;
            LastInstallationSuccessDateUtc = installationUtc;
        }
        public bool IsServiceEnabled { get; }
        public DateTime? LastSearchSuccessDateUtc { get; }
        public DateTime? LastInstallationSuccessDateUtc { get; }
        public DateTime? LastSearchSuccessDate { get { return this.LastSearchSuccessDateUtc?.ToLocalTime(); } } 
        public DateTime? LastInstallationSuccessDate { get { return this.LastInstallationSuccessDateUtc?.ToLocalTime(); } } 
    }
}