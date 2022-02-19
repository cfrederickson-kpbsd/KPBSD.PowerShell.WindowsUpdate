using System;
using System.Runtime.InteropServices;

namespace KPBSD.PowerShell.WindowsUpdate
{
    // Definitions for all COM interfaces I use in this library

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
}