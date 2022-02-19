using System;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    // Definitions for all COM interfaces I use in this library

    #region Search
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("04c6895d-eaf2-4034-97f3-311de9be413a")]
    public interface IUpdateSearcher
    {
        ISearchJob BeginSearch(string criteria, ISearchCompletedCallback callback, object state);
        object EndSearch(ISearchJob searchJob);
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("88AEE058-D4B0-4725-A2F1-814A67AE964C")]
    public interface ISearchCompletedCallback
    {
        void Invoke(ISearchJob job, ISearchCompletedCallbackArgs callbackArgs);
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("7366EA16-7A1A-4EA2-B042-973D3E9CD99B")]
    public interface ISearchJob
    {
        bool IsCompleted { get; }
        object AsyncState { get; }
        void CleanUp();
        void RequestAbort();
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A700A634-2850-4C47-938A-9E4B6E5AF9A6")]
    public interface ISearchCompletedCallbackArgs
    {
    }

    #endregion
    
    #region Download
    // Note to self: we only need to expose the bare minimum that is required to write progress updates,
    // job results, and errors, as well as to fire off the delegate itself.
    // Realistically, if I just stick with using dynamic (almost) everywhere, I can only write up
    // the delegate interfaces and parameters.

    public interface IUpdateDownloadResult {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
    }
    public interface IDownloadProgress {
        decimal CurrentUpdateBytesDownloaded { get; }
        decimal CurrentUpdateBytesToDownload { get; }
        DownloadPhase CurrentUpdateDownloadPhase { get; }
        long CurrentUpdateIndex { get; }
        long CurrentUpdatePercentComplete { get; }
        long PercentComplete { get; }
        decimal TotalBytesDownloaded { get; }
        decimal TotalBytesToDownload { get; }
        IUpdateDownloadResult GetUpdateResult(long updateIndex);
    }
    public interface IDownloadJob {
        object AsyncState {get;}
        bool IsCompleted { get; }
        dynamic Updates { get; }
        IDownloadProgress GetProgress();
        void RequestAbort();
    }
    public interface IDownloadProgressChangedCallbackArgs {
        IDownloadProgress Progress { get; }
    }
    public interface IDownloadProgressChangedCallback {
        void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs);
    }
    public interface IDownloadCompletedCallbackArgs {

    }
    public interface IDownloadCompletedCallback {
        void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs);
    }
    public interface IDownloadResult {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
        IUpdateDownloadResult GetUpdateResult(long updateIndex);
    }
    public interface IUpdateDownloader {
        IDownloadJob BeginDownload(IDownloadProgressChangedCallback onProgress, IDownloadCompletedCallback onCompleted, object state);
        IDownloadResult EndDownload(IDownloadJob downloadJob);
    }

    #endregion

    #region Install

    #endregion
}