using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScreensaverFocus {
    static class Program {
        public static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppName = AppDomain.CurrentDomain.FriendlyName;
        public static readonly string AppNameExe = AppDomain.CurrentDomain.FriendlyName+".exe";
        public static readonly string AppFullPath = AppDirectory+AppNameExe;
        public static readonly string AppChildExe = "ScreensaverProfile";

        public static readonly string CommandRemoveStartup = "-u";

        [STAThread]
        static void Main(string[] args) {
            if (CheckIfRunning())
                return;

            if (args.Length > 0) {
                bool exit = HandleCommands(args);
                if (exit) return;
            }
            else {
                // Normal application execution, no commands
                AddOrUpdateStartup();
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new TrayService());
        }


        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /*
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        */

        private static bool HandleCommands(string[] args) {
            bool exit = false;
            if (args[0].Equals(CommandRemoveStartup)) {
                // Command to remove startup
                bool removed = RemoveStartup();
                bool attached = AttachConsole(ATTACH_PARENT_PROCESS);
                if (attached) {
                    // a bit bugged/hacked atm, but i want to use the current console and for now its the way i found out
                    Console.WriteLine("\n");
                    if (removed)
                        Console.WriteLine("ScreensaverFocus auto startup removed.");
                    else
                        Console.WriteLine(
                            "Could not find auto startup, maybe was already removed?\n" +
                            "You can manually check it by using 'Task Scheduler', and then see if '"+AppName+"' is present in 'Task Scheduler Library' root."
                        );
                    SendKeys.SendWait("{ENTER}");
                }
                exit = true;
            }
            return exit;
        }

        private static bool CheckIfRunning() {
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 1) {
                MessageBox.Show("This application is already running.", AppName);
                return true;
            }
            return false;
        }

        private static void AddOrUpdateStartup() {
            using (TaskService taskService = new TaskService()) {
                bool taskExists = taskService.RootFolder.Tasks.Exists(AppName);

                // Check if the path has changed, and update the startup task / create new one
                if (taskExists) {
                    using (TaskService ts = new TaskService()) {
                        var task = ts.GetTask(AppName);
                        using (ActionCollection a = task.Definition.Actions) {
                            var taskActionPath = a.First().ToString();
                            if (!taskActionPath.Equals(AppFullPath)) {
                                ts.RootFolder.DeleteTask(AppName);
                                taskExists = false;
                            }
                        }
                    }
                }

                if (!taskExists) {
                    TaskDefinition taskDefinition = taskService.NewTask();

                    // Set the task properties
                    taskDefinition.RegistrationInfo.Description = AppName;
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                    taskDefinition.Triggers.Add(new LogonTrigger());
                    taskDefinition.Actions.Add(new ExecAction(AppDirectory+AppNameExe));

                    // Register the task with the Task Scheduler
                    taskService.RootFolder.RegisterTaskDefinition(AppName, taskDefinition);
                }
            }
        }

        private static bool RemoveStartup() {
            bool removed = false;
            using (TaskService taskService = new TaskService()) {
                bool taskExists = taskService.RootFolder.Tasks.Exists(AppName);

                if (taskExists) {
                    using (TaskService ts = new TaskService()) {
                        var task = ts.GetTask(AppName);
                        ts.RootFolder.DeleteTask(AppName);
                        removed = true;
                    }
                }
            }
            return removed;
        }
    }
}