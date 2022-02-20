using System;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    // Definitions for all COM interfaces I use in this library

    #region Search
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

    // [ComImport]
    // [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateDownloadResult {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
    }
    // [ComImport]
    // [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
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
    [ComImport]
        [Guid("C574DE85-7358-43F6-AAE8-8697E62D8BA7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadJob {
        object AsyncState {get;}
        bool IsCompleted { get; }
        dynamic Updates { get; }
        IDownloadProgress GetProgress();
        void RequestAbort();
    }
    [ComImport]
        [Guid("324FF2C6-4981-4B04-9412-57481745AB24")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadProgressChangedCallbackArgs {
        IDownloadProgress Progress { get; }
    }
    [ComImport]
    [Guid("8C3F1CDD-6173-4591-AEBD-A56A53CA77C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDownloadProgressChangedCallback {
        void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs);
    }
    [ComImport]
        [Guid("FA565B23-498C-47A0-979D-E7D5B1813360")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadCompletedCallbackArgs {

    }
    [ComImport]
        [Guid("77254866-9F5B-4C8E-B9E2-C77A8530D64B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDownloadCompletedCallback {
        void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs);
    }
    [ComImport]
        [Guid("DAA4FDD0-4727-4DBE-A1E7-745DCA317144")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadResult {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
        IUpdateDownloadResult GetUpdateResult(long updateIndex);
    }

    #endregion

    #region Install

    #endregion
}