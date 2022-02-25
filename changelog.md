# 1.0
Initial release. Includes the following commands:
- [x] Get-WindowsUpdateHistory
- [x] Get-WindowsUpdate
- [x] Request-WindowsUpdate (Download-WindowsUpdate)
- [x] Install-WindowsUpdate
- [x] Uninstall-WindowsUpdate
- [x] Resume-WindowsAutomaticUpdate
- [ ] Suspend-WindowsAutomaticUpdate
- [ ] Start-WindowsAutomaticUpdate
- [ ] Test-WindowsUpdateRebootRequired
- [ ] Hide-WindowsUpdate
- [ ] Show-WindowsUpdate
- [ ] Set-WindowsAutomaticUpdateSetting
- [ ] Export-WindowsUpdate
[//]: # Calls `$WindowsUpdate.ComObject.CopyFromCache($path, $bool)`. Currently not implemented because I need to figure out what to do about bundled updates and the not downloaded errors I keep getting.
- [ ] Import-WindowsUpdate
[//]: # Converse of `Export-WindowsUpdate`. Calls `$WindowsUpdate.ComObject.CopyToCache($files)`. Not sure how to get the update to copy from the cache and how all that works out yet, so also not implemented.


TODO:
- [ ] Combine all WindowsUpdateJob implementations into a single class which is able to begin the next step for each operation at the time that a previous operation completes.
 - [ ] e.g. search, when search completes download, when download completes install
 - [ ] it may be necessary to explicitly include bundled updates in the installation step; my installs aren't seeming to work right
 - [ ] dispose should wait for CleanUp() to finish, probably using a wait handle.