namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;

    internal class ComDelegate<T1, T2> :
            ISearchCompletedCallback,
            IDownloadCompletedCallback,
            IInstallationCompletedCallback,
            IDownloadProgressChangedCallback,
            IInstallationProgressChangedCallback
        {
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
}