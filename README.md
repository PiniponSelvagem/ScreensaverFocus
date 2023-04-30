# ScreensaverFocus
Since most, if not all, RGB programs on the market dont have an option to detect a running scrensaver, this app adds support for them to do just that.

The only UI it has is on the taskbar, and can be accessed by right click on the icon.

# Tested with:
- Corsair iCUE
- SteelSeries Engine

Should support any software that auto switches profiles based on the focused window.

# Functionality
- Runs in background.
- No 'active waits' used, just events, this means no unnecessary CPU cycles to check if the screensaver is running.
- Auto startup set up by using the taskscheduler, upon starting the app.
- Easy autostart remove.

# Demo
Youtube: https://www.youtube.com/watch?v=ydC8lk04h2Q

![Context menu](https://raw.githubusercontent.com/PiniponSelvagem/ScreensaverFocus/main/wiki/images/ssf_context_menu.png)

![iCue profile](https://raw.githubusercontent.com/PiniponSelvagem/ScreensaverFocus/main/wiki/images/icue_profile.png)

# Setup
Using your desired RGB software:
1. Create or using an existing profile, add the EXE 'ScreensaverProfile.exe' to it.
2. And thats it, xD

## NOTE:
Currently, 'ScreensaverFocus' does not detect the modification of the screensaver settings, this means that if you change to a different screensaver, it won't detect it. In order for 'ScreensaverFocus' detect the new screensaver:
1. Right-click 'ScreensaverFocus' at the system tray.
2. Select 'Update screensaver'.

Also, it can happen that some screensavers at the Screensaver Windows settings can trigger 'ScreensaverFocus'. You can temporarily disable this, by:
1. Right-click 'ScreensaverFocus' at the system tray.
2. Select 'Pause'
3. To activate again, just click on 'Resume'.

# Remove from system startup
### Method 1
Use the built in command, and accept the administrator privileges:
```
ScreensaverFocus.exe -u
```
### Method 2
1. Open start menu and type: 'Task Scheduler'
2. Opening 'Task Scheduler' will show on a list on teh left side with: 'Task Scheduler (Local)' and 'Task Scheduler Library', select 'Task Scheduler Library'.
3. At the center of the main window, you will see a list of scheduled tasks, find 'ScreensaverFocus'.
4. Right-click on 'ScreensaverFocus', a context menu shows up, and click 'Delete'.
5. Done!

# FAQ
Why does it require administrator privileges?
- Adding a taskschedule. This is used to set up auto start on system boot.
- Getting events of start/stop processes. This is used for the app get notifications whena  process starts and stops, in this case the Screensaver. Removing administrator priviliges could be done by using 'Process.GetProcesses()', but this would require a periodic wake up of the app which this app tries to avoid.
- Checking the selected screensaver from Windows Registry at:
```
    key: HKEY_CURRENT_USER\Control Panel\Desktop
    subkey: SCRNSAVE.EXE
```