# 1.0
Initial release. Includes the following commands:
- [x] Get-WindowsUpdateHistory
- [x] Get-WindowsUpdate
- [x] Request-WindowsUpdate (Download-WindowsUpdate)
- [x] Install-WindowsUpdate
- [ ] Uninstall-WindowsUpdate
- [x] Clear-WindowsUpdateDownloadCache

# Planned Updates
Not that upcoming features in this list are not guaranteed to ever be implemented. This is more of a "to do" list of ideas for the module.

### 1.0.1
- placeholder

## 1.1
- [ ] Export-WindowsUpdate
[//]: Calls `$WindowsUpdate.ComObject.CopyFromCache('path', $bool)`. Currently not implemented because I need to figure out what to do about bundled updates and the not downloaded errors I keep getting.
- [ ] Import-WindowsUpdate
[//]: # Converse of `Export-WindowsUpdate`. Calls `$WindowsUpdate.ComObject.CopyToCache('path', $bool)`. Not sure how to get the update to copy from the cache and how all that works out yet, so also not implemented.

