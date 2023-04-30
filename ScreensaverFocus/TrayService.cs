using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreensaverFocus {
    public partial class TrayService : Form {

        private static readonly Screensaver screensaver = new Screensaver();
        private static readonly Mutex mutex = new Mutex(false, "ScreensaverFocusMutex");

        private static readonly List<Process> pList = new List<Process>();

        private static readonly string PAUSE = "Pause";
        private static readonly string RESUME = "Resume";

        private static bool status = true;
        private static ProcessWatcherStart watcherStart = new ProcessWatcherStart();
        private static ProcessWatcherStop watcherStop   = new ProcessWatcherStop();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public TrayService() {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        private void SSFtray_Load(object sender, EventArgs e) {
            Hide();
            notifyIcon.Text = Program.AppName;

            // Context menu
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem itemUpdate = new ToolStripMenuItem("Update screensaver");
            ToolStripMenuItem itemToggle = new ToolStripMenuItem(PAUSE);
            ToolStripMenuItem itemExit = new ToolStripMenuItem("Exit");
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { itemUpdate, itemToggle, itemExit });
            itemUpdate.Click += UpdateScreensaver;
            itemToggle.Click += ToggleStatus;
            itemExit.Click += Exit;

            // assign the ContextMenuStrip to the NotifyIcon control
            notifyIcon.ContextMenuStrip = contextMenuStrip;

            mutex.WaitOne();
            
            watcherStart.EventHandle += EventProcessStart;
            watcherStop.EventHandle += EventProcessStop;
        }

        void EventProcessStart(object sender, string processName) {
            if (processName.Contains(screensaver.Name)) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = Program.AppChildExe,
                    Arguments = mutex.SafeWaitHandle.DangerousGetHandle().ToInt32().ToString()
                };

                Process p = new Process {
                    StartInfo = startInfo
                };
                try {
                    p.Start();
                    pList.Add(p);
                }
                catch (Exception) {
                    notifyIcon.Text = "Screensaver Focus";
                    notifyIcon.BalloonTipTitle = "Screensaver Focus";
                    notifyIcon.BalloonTipText = "Could not find 'ScreensaverProfile.exe'!\n" +
                        "Make sure 'ScreensaverProfile.exe' and 'ScreensaverFocus.exe' are inside the same folder.";
                    notifyIcon.ShowBalloonTip(5000);
                }

                var process = Process.GetProcessesByName("ScreensaverProfile");
                if (process.Length > 0)
                    SetForegroundWindow(process[0].MainWindowHandle);
            }
        }

        void EventProcessStop(object sender, string processName) {
            if (processName.Contains(screensaver.Name)) {
                try {
                    Process p = pList.First();
                    p.Kill();
                    pList.Remove(p);
                }
                catch (Exception) {
                    // Abafador ;)
                }
            }
        }

        void UpdateScreensaver(object sender, EventArgs e) {
            screensaver.Update();
        }

        void ToggleStatus(object sender, EventArgs e) {
            ToolStripMenuItem? item = sender as ToolStripMenuItem;
            if (item != null) {
                if (status) {
                    item.Text = RESUME;
                    status = false;
                    watcherStart.EventHandle -= EventProcessStart;
                    watcherStop.EventHandle -= EventProcessStop;

                    pList.ForEach(p => {
                        try {
                            p.Kill();
                        }
                        catch (Exception) {
                            // Abafador ;)
                        }
                    });
                    pList.Clear();
                }
                else {
                    item.Text = PAUSE;
                    status = true;
                    watcherStart.EventHandle += EventProcessStart;
                    watcherStop.EventHandle += EventProcessStop;
                }
             }
        }

        void Exit(object sender, EventArgs e) {
            notifyIcon.Dispose();
            Close();
        }
    }
}
