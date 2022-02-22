# KPBSD.PowerShell.WindowsUpdate

## Synopsis
PowerShell module for downloading and installing Windows Updates on client devices.

## Description
This module is geared towards Windows environments using managed updates that want the ability to
programatically manage Windows Updates. The module works well with WSUS and provides the ability
to download and install updates by a script-driven process instead of relying on the deadline
feature of WSUS updates to push updates.

For additional information, see the module help document.

## Continued Support
Once this module is in a functional state I'm not likely to spend too much time focusing
on extending support or adding functionality for some less-often-used features in the
WUApiLib COM library. If others have features you'd like to be available, feel free to
open a pull request.

That being said, this was my first project where I focued on testing (and more specifically,
actually wrote tests for more than one or two commands). I believe it's helped me to be more
organized and consistent. However, I've had some trouble coming up with tests for this
module since the core functionality is intended to work with Windows Updates which will
vary over time or by system. Hence, I have tests for helper functions and whatnot but I feel
that the actual exported commands are lacking in available tests. If anyone has ideas for
additional tests I can write that would be able to be run without actually downloading,
installing, or uninstalling an update, I'd appreciate your input.