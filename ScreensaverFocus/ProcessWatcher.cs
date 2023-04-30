using System.Management;

namespace ScreensaverFocus {

    abstract class ProcessWatcher {
        public event EventHandler<string>? EventHandle;

        public void DoAction(object sender, EventArrivedEventArgs eventArgs) {
            var processName = eventArgs.NewEvent.Properties["ProcessName"].Value.ToString();
            if (processName != null)
                EventHandle?.Invoke(this, processName);
        }
    }

    class ProcessWatcherStart : ProcessWatcher {
        public ProcessWatcherStart() {
            var watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            watcher.EventArrived += (sender, eventArgs) => { DoAction(sender, eventArgs); };
            watcher.Start();
        }
    }

    class ProcessWatcherStop : ProcessWatcher {
        public ProcessWatcherStop() {
            var watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            watcher.EventArrived += (sender, eventArgs) => { DoAction(sender, eventArgs); };
            watcher.Start();
        }
    }
}
