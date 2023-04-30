# ScreensaverFocus
Since most, if not all, RGB programs on the market dont have an option to detect a running scrensaver, this app adds support for them to do just that.

The only UI it has is on the taskbar, and can be accessed by right click on the icon.

# Functionality
- Runs in background.
- No 'active waits' used, just events, this means no unnecessary CPU cycles to check if the screensaver is running.
- Auto startup set up by using the taskscheduler.

# FAQ
Why does it require administrator privileges?
- Adding a taskschedule. This is used to set up auto start on system boot.
- Getting events of start/stop processes. This is used for the app get notifications whena  process starts and stops, in this case the Screensaver. Removing administrator priviliges could be done by using 'Process.GetProcesses()', but this would require a periodic wake up of the app which this app tries to avoid.
- Checking the selected screensaver from Windows Registry at:
```
    key: HKEY_CURRENT_USER\Control Panel\Desktop
    subkey: SCRNSAVE.EXE
```