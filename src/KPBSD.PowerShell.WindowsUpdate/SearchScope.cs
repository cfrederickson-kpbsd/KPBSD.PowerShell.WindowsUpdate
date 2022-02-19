namespace KPBSD.PowerShell.WindowsUpdate
{
    public enum SearchScope {
        Default = 0,
        MachineOnly = 1,
        CurrentUserOnly = 2,
        MachineAndCurrentUser = 3,
        MachineAndAllUsers = 4,
        AllUsers = 5
    }
}