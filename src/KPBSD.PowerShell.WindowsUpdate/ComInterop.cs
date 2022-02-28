using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    // Note to self: we only need to expose the bare minimum that is required to write progress updates,
    // job results, and errors, as well as to fire off the delegate itself.
    // Realistically, if I just stick with using dynamic (almost) everywhere, I can only write up
    // the delegate interfaces and parameters.

    #region General
    [ComImport, Guid("46297823-9940-4C09-AED9-CD3EA6D05968")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateIdentity
    {
        string UpdateID { get; }
        long RevisionNumber { get; }
    }
    [ComImport, Guid("EFF90582-2DDC-480F-A06D-60F3FBC362C3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IStringCollection : IEnumerable
    {
        long Add(string value);
        void Clear();
        long Count { get; }
        string this[long index] { get; set; }
        bool ReadOnly { get; }
        void Insert(long index, string value);
        void RemoveAt(long index);
    }
    [ComImport, Guid("6A92B07A-D821-4682-B423-5C805022CC4D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdate
    {
        IUpdateCollection BundledUpdates { get; }
        ICategoryCollection Categories { get; }
        DateTime Deadline { get; }
        DeploymentAction DeploymentAction { get; }
        DownloadPriority DownloadPriority { get; }
        bool EulaAccepted { get; }
        IUpdateIdentity Identity { get; }
        bool IsDownloaded { get; }
        bool IsHidden { get; }
        bool IsInstalled { get; }
        bool IsMandatory { get; }
        bool IsUninstallable { get; }
        IStringCollection KBArticleIDs { get; }
        string Title { get; }
        UpdateType Type { get; }
    }
    [ComImport, Guid("81DDC1B8-9D35-47A6-B471-5B80F519223B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICategory
    {

    }
    [ComImport, Guid("07f7438c-7709-4ca5-b518-91279288134e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateCollection : IEnumerable
    {
        long Add(IUpdate update);
        void Clear();
        IUpdateCollection Copy();
        long Count { get; }
        IUpdate this[long index] { get; set; }
        bool ReadOnly { get; set; }
        void Insert(long index, IUpdate value);
        void RemoveAt(long index);
    }
    [ComImport]
    [Guid("07f7438c-7709-4ca5-b518-91279288134e")]
    public class UpdateCollection
    {
    }
    [ComImport, Guid("918efd1e-b5d8-4c90-8540-aeb9bdc56f9d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateSession
    {
        IUpdateDownloader CreateUpdateDownloader();
        IUpdateInstaller CreateUpdateInstaller();
        IUpdateSearcher CreateUpdateSearcher();
    }
    [ComImport, Guid("3a56bfb8-576c-43f7-9335-fe4838fd7e37")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICategoryCollection : IEnumerable
    {
        long Count { get; }
        ICategory this[long index] { get; }
    }
    [ComImport, Guid("a376dd5e-09d4-427f-af7c-fed5b6e1c1d6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateException
    {
        UpdateExceptionContext Context { get; }
        long HResult { get; }
        string Message { get; }
    }
    [ComImport, Guid("503626a3-8e14-4729-9355-0fe664bd2321")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateExceptionCollection : IEnumerable
    {
        long Count { get; }
        IUpdateException this[long index] { get; }
    }
    #endregion


    #region Search
    [ComImport, Guid("04c6895d-eaf2-4034-97f3-311de9be413a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateSearcher
    {
        ISearchJob BeginSearch(string criteria, ISearchCompletedCallback onCompleted, object? state);
        ISearchResult EndSearch(ISearchJob searchJob);
        string EscapeString (string inputString);
        bool IncludePotentiallySupersededUpdates { get; set; }
        bool Online { get; set; }
        ServerSelection ServerSelection { get; set; }
        string ServiceID { get; set; }
        long GetTotalHistoryCount();
        bool CanAutomaticallyUpgradeService { get; set; }
    }
    [ComImport, Guid("d40cff62-e08c-4498-941a-01e25f0fd33c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ISearchResult
    {
        OperationResultCode ResultCode { get; }
        ICategoryCollection RootCategories { get; }
        IUpdateCollection Updates { get; }
        IUpdateExceptionCollection Warnings { get; }
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("88AEE058-D4B0-4725-A2F1-814A67AE964C")]
    public interface ISearchCompletedCallback
    {
        void Invoke(ISearchJob job, ISearchCompletedCallbackArgs callbackArgs);
    }
    [ComImport, Guid("7366EA16-7A1A-4EA2-B042-973D3E9CD99B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
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
    [ComImport, Guid("68f1c6f9-7ecc-4666-a464-247fe12496c3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateDownloader
    {
        IDownloadJob BeginDownload(IDownloadProgressChangedCallback? onProgressChanged, IDownloadCompletedCallback onCompleted, object? state);
        IDownloadResult EndDownload(IDownloadJob downloadJob);
        bool IsForced { get; set; }
        DownloadPriority Priority { get; set; }
        IUpdateCollection Updates { get; set; }
    }

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
        IUpdateCollection Updates { get; }
        IDownloadProgress GetProgress();
        void RequestAbort();
        void CleanUp();
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
    [ComImport, Guid("ef8208ea-2304-492d-9109-23813b0958e1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUpdateInstaller
    {
        IInstallationJob BeginInstall(IInstallationProgressChangedCallback? onProgressChanged, IInstallationCompletedCallback onCompleted, object? state);
        IInstallationResult EndInstall(IInstallationJob installationjob);
        IInstallationJob BeginUninstall(IInstallationProgressChangedCallback? onProgressChanged, IInstallationCompletedCallback onCompleted, object? state);
        IInstallationResult EndUninstall(IInstallationJob uninstallationJob);
        bool AllowSourcePrompts { get; set; }
        /// <summary>
        /// An installation or uninstallation is in progress a computer at the moment. When an installation or
        /// uninstallation is in progress, the new installation or uninstallation immediately fails with the WU_E_OPERATIONINPROGRESS
        /// error. The IsBusy property does not secure an opportunity for the caller to begin a new installation or
        /// uninstallation. if the IsBusy property or a recent installation or uninstallation failure indicates that another installation
        /// or uninstallation is already in progress, the caller should attempt the installation or uninstallation later.
        /// </summary>
        /// <value></value>
        bool IsBusy { get; }
        /// <summary>
        /// Forcibly install or uninstall a update (even if it is already installed or uninstalled).
        /// Forced installation fails if the update is not already installed.
        /// </summary>
        /// <value></value>
        bool IsForced { get; set; }
        bool RebootRequiredBeforeInstallation { get; }
        IUpdateCollection Updates { get; set; }
    }
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
        IUpdateCollection Updates { get; }
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