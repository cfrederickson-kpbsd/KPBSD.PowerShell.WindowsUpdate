namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    public static class WindowsUpdateExtensions
    {
        public static Task<ISearchResult> SearchAsync(PSObject searcher, string criteria, CancellationToken cancellationToken = default)
        {
            if (searcher.BaseObject is IUpdateSearcher i)
            {
                return i.SearchAsync(criteria, cancellationToken);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public static async Task<ISearchResult> SearchAsync(this IUpdateSearcher searcher, string criteria, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<ISearchResult>();
            var onCompleted = new ComDelegate<ISearchJob, ISearchCompletedCallbackArgs>((job, callbackArgs) =>
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        var searcher = (IUpdateSearcher)job.AsyncState;
                        var searchResult = searcher.EndSearch(job);
                        if (searchResult.ResultCode == OperationResultCode.Aborted)
                        {
                            tcs.SetCanceled();
                        }
                        else
                        {
                            tcs.SetResult(searchResult);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                    finally
                    {
                        job.CleanUp();
                    }
                });
            });
            var job = searcher.BeginSearch(criteria, onCompleted, searcher);
            using var _ = cancellationToken.Register(job.RequestAbort);
            return await tcs.Task.ConfigureAwait(false);
        }
        public static async Task<JobResult<IDownloadResult>> DownloadAsync(this IUpdateDownloader downloader, Action<IDownloadJob, IDownloadProgressChangedCallbackArgs>? onProgress, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<IDownloadResult>();
            ComDelegate<IDownloadJob, IDownloadProgressChangedCallbackArgs>? onProgressDelegate = null;
            if (onProgress == null)
            {
                onProgressDelegate = null;
            }
            else
            {
                onProgressDelegate = new ComDelegate<IDownloadJob, IDownloadProgressChangedCallbackArgs>(onProgress);
            }
            var onCompleted = new ComDelegate<IDownloadJob, IDownloadCompletedCallbackArgs>((job, callbackArgs) =>
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        var downloader = (IUpdateDownloader)job.AsyncState;
                        var result = downloader.EndDownload(job);
                        if (result.ResultCode == OperationResultCode.Aborted)
                        {
                            tcs.SetCanceled();
                        }
                        else if (result.ResultCode == OperationResultCode.Failed
                            && ComErrorCodes.TryGetErrorDetails(result.HResult, out var errorId, out var errorCategory, out var message))
                        {
                            var exn = new COMException(message, result.HResult);
                            exn.Data["errorId"] = errorId;
                            exn.Data["errorCategory"] = errorCategory;
                            tcs.SetException(exn);
                        }
                        else
                        {
                            tcs.SetResult(result);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                    finally
                    {
                        job.CleanUp();
                    }
                });
            });
            var job = downloader.BeginDownload(onProgressDelegate, onCompleted, downloader);
            using var _ = cancellationToken.Register(job.RequestAbort);
            return new JobResult<IDownloadResult>(await tcs.Task.ConfigureAwait(false), job.Updates);
        }
        public static async Task<JobResult<IInstallationResult>> InstallAsync(this IUpdateInstaller installer, Action<IInstallationJob, IInstallationProgressChangedCallbackArgs>? onProgress, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<IInstallationResult>();
            ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>? onProgressDelegate = null;
            if (onProgress == null)
            {
                onProgressDelegate = null;
            }
            else
            {
                onProgressDelegate = new ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>(onProgress);
            }
            var onCompleted = new ComDelegate<IInstallationJob, IInstallationCompletedCallbackArgs>((job, callbackArgs) =>
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        var downloader = (IUpdateInstaller)job.AsyncState;
                        var result = downloader.EndInstall(job);
                        if (result.ResultCode == OperationResultCode.Aborted)
                        {
                            tcs.SetCanceled();
                        }
                        else if (result.ResultCode == OperationResultCode.Failed
                            && ComErrorCodes.TryGetErrorDetails(result.HResult, out var errorId, out var errorCategory, out var message))
                        {
                            var exn = new COMException(message, result.HResult);
                            exn.Data["errorId"] = errorId;
                            exn.Data["errorCategory"] = errorCategory;
                            tcs.SetException(exn);
                        }
                        else
                        {
                            tcs.SetResult(result);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                    finally
                    {
                        job.CleanUp();
                    }
                });
            });
            var job = installer.BeginInstall(onProgressDelegate, onCompleted, installer);
            using var _ = cancellationToken.Register(job.RequestAbort);
            return new JobResult<IInstallationResult>(await tcs.Task.ConfigureAwait(false), job.Updates);
        }

        public static async Task<JobResult<IInstallationResult>> UninstallAsync(this IUpdateInstaller installer, Action<IInstallationJob, IInstallationProgressChangedCallbackArgs>? onProgress, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<IInstallationResult>();
            ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>? onProgressDelegate = null;
            if (onProgress == null)
            {
                onProgressDelegate = null;
            }
            else
            {
                onProgressDelegate = new ComDelegate<IInstallationJob, IInstallationProgressChangedCallbackArgs>(onProgress);
            }
            var onCompleted = new ComDelegate<IInstallationJob, IInstallationCompletedCallbackArgs>((job, callbackArgs) =>
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        var downloader = (IUpdateInstaller)job.AsyncState;
                        var result = downloader.EndUninstall(job);
                        if (result.ResultCode == OperationResultCode.Aborted)
                        {
                            tcs.SetCanceled();
                        }
                        else if (result.ResultCode == OperationResultCode.Failed
                            && ComErrorCodes.TryGetErrorDetails(result.HResult, out var errorId, out var errorCategory, out var message))
                        {
                            var exn = new COMException(message, result.HResult);
                            exn.Data["errorId"] = errorId;
                            exn.Data["errorCategory"] = errorCategory;
                            tcs.SetException(exn);
                        }
                        else
                        {
                            tcs.SetResult(result);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                    finally
                    {
                        job.CleanUp();
                    }
                });
            });
            var job = installer.BeginUninstall(onProgressDelegate, onCompleted, installer);
            using var _ = cancellationToken.Register(job.RequestAbort);
            return new JobResult<IInstallationResult>(await tcs.Task.ConfigureAwait(false), job.Updates);
        }
    }
}