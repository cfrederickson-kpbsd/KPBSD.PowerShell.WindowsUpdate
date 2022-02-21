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

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("BF99AF76-B575-42AD-8AA4-33CBB5477AF1")]
    public interface IUpdateDownloadResult
    {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
    }
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("D31A5BAC-F719-4178-9DBB-5E2CB47FD18A")]
    public interface IDownloadProgress
    {
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
    public interface IDownloadJob
    {
        object AsyncState { get; }
        bool IsCompleted { get; }
        dynamic Updates { get; }
        IDownloadProgress GetProgress();
        void RequestAbort();
    }
    [ComImport]
    [Guid("324FF2C6-4981-4B04-9412-57481745AB24")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadProgressChangedCallbackArgs
    {
        IDownloadProgress Progress { get; }
    }
    [ComImport]
    [Guid("8C3F1CDD-6173-4591-AEBD-A56A53CA77C1")]

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDownloadProgressChangedCallback
    {
        void Invoke(IDownloadJob downloadJob, IDownloadProgressChangedCallbackArgs callbackArgs);
    }
    [ComImport]
    [Guid("FA565B23-498C-47A0-979D-E7D5B1813360")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadCompletedCallbackArgs
    {

    }
    [ComImport]
    [Guid("77254866-9F5B-4C8E-B9E2-C77A8530D64B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDownloadCompletedCallback
    {
        void Invoke(IDownloadJob downloadJob, IDownloadCompletedCallbackArgs callbackArgs);
    }
    [ComImport]
    [Guid("DAA4FDD0-4727-4DBE-A1E7-745DCA317144")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDownloadResult
    {
        int HResult { get; }
        OperationResultCode ResultCode { get; }
        IUpdateDownloadResult GetUpdateResult(long updateIndex);
    }

    #endregion

    #region Install
    
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("45F4F6F3-D602-4F98-9A8A-3EFA152AD2D3")]
    public interface IInstallationCompletedCallback
    {
        void Invoke(IInstallationJob job, IInstallationCompletedCallbackArgs callbackArgs);
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("5C209F0B-BAD5-432A-9556-4699BED2638A")]
    public interface IInstallationJob {
        void CleanUp();
        IInstallationProgress GetProgress();
        void RequestAbort();
        object AsyncState { get; }
        bool IsCompleted { get; }
        dynamic Updates { get; }
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("250E2106-8EFB-4705-9653-EF13C581B6A1")]
    public interface IInstallationCompletedCallbackArgs {}
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("E01402D5-F8DA-43BA-A012-38894BD048F1")]
    public interface IInstallationProgressChangedCallback
    {
        void Invoke(IInstallationJob job, IInstallationProgressChangedCallbackArgs callbackArgs);
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("345C8244-43A3-4E32-A368-65F073B76F36")]
    public interface IInstallationProgress
    {
        long CurrentUpdateIndex { get; }
        long CurrentUpdatePercentComplete { get; }
        long PercentComplete { get; }
        IUpdateInstallationResult GetUpdateResult(long updateIndex);
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("E4F14E1E-689D-4218-A0B9-BC189C484A01")]
    public interface IInstallationProgressChangedCallbackArgs {
        IInstallationProgress Progress { get; }
    }
    
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("D940F0F8-3CBB-4FD0-993F-471E7F2328AD")]
    public interface IUpdateInstallationResult
    {
        public int HResult { get; }
        public int RebootRequired { get; }
        public OperationResultCode ResultCode { get; }
    }
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A43C56D6-7451-48D4-AF96-B6CD2D0D9B7A")]
    public interface IInstallationResult {
        int HResult { get; }
        bool RebootRequired { get; }
        OperationResultCode ResultCode { get; }
        IUpdateInstallationResult GetUpdateResult(long updateIndex);
    }
    #endregion
}