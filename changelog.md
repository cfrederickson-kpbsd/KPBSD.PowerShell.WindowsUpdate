# 1.0
Initial release. Includes the following commands:
- [x] Get-WindowsUpdateHistory
- [x] Get-WindowsUpdate
- [x] Request-WindowsUpdate (Download-WindowsUpdate)
- [x] Install-WindowsUpdate
- [ ] Uninstall-WindowsUpdate
- [ ] Export-WindowsUpdate
[//]: Calls `$WindowsUpdate.ComObject.CopyFromCache($path, $bool)`. Currently not implemented because I need to figure out what to do about bundled updates and the not downloaded errors I keep getting.
- [ ] Import-WindowsUpdate
[//]: # Converse of `Export-WindowsUpdate`. Calls `$WindowsUpdate.ComObject.CopyToCache($files)`. Not sure how to get the update to copy from the cache and how all that works out yet, so also not implemented.
