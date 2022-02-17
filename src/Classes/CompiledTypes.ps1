if (-not ('KPBSD.PowerShell.WindowsUpdate.ServerSelection' -as [Type])) {
    Add-Type -TypeDefinition @'
namespace KPBSD.PowerShell.WindowsUpdate {
    public enum ServerSelection {
        Default = 0,
        ManagedServer = 1,
        WindowsUpdate = 2,
        Others = 3
    }
    public enum OperationType {
        Installation = 1,
        Uninstallation = 2
    }
}
'@
}
