using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;

namespace ScreensaverProfile {
    public partial class HiddenWindow : Form {

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenMutex(int dwDesiredAccess, bool bInheritHandle, string lpName);


        public HiddenWindow() {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Set transparent background
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            // Remove window border
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
        }

        private void OnShow(object sender, EventArgs e) {
            Mutex? mutex = null;
            try {
                mutex = Mutex.OpenExisting("ScreensaverFocusMutex");
                mutex.WaitOne();
            } 
            catch(Exception) { }
            finally {
                try {
                    mutex?.ReleaseMutex();
                } catch (Exception) { }
                Close();
            }
        }
    }
}