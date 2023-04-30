using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreensaverFocus {
    class Screensaver {
        private static readonly string KEY = "Control Panel\\Desktop";
        private static readonly string SUBKEY = "SCRNSAVE.EXE";

        public string Name { get; private set; }

        public Screensaver() {
            Name = string.Empty;
            Update();
        }

        public void Update() {
            var key = Registry.CurrentUser.OpenSubKey(KEY, false);
            if (key != null) {
                var subkey = key.GetValue(SUBKEY);
                string? screensaverPath = subkey != null ? subkey.ToString() : string.Empty;
                string? name = Path.GetFileNameWithoutExtension(screensaverPath);
                if (name != null)
                    Name = name;
                key.Close();
            }
        }
    }
}
