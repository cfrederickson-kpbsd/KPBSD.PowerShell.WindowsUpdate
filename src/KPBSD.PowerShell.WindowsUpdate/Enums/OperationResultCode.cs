namespace KPBSD.PowerShell.WindowsUpdate
{
    public enum OperationResultCode {
        NotStarted = 0,
        InProgress = 1,
        Succeeded = 2,
        SucceededWithErrors = 3,
        Failed = 4,
        Aborted = 5
    }
}