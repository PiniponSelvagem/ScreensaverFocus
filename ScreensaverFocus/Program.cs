using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Threading.Tasks;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace ScreensaverFocus {
    static class Program {
        public static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppName = AppDomain.CurrentDomain.FriendlyName;
        public static readonly string AppNameExe = AppDomain.CurrentDomain.FriendlyName+".exe";
        public static readonly string AppFullPath = AppDirectory+AppNameExe;
        public static readonly string AppChildExe = "ScreensaverProfile";

        [STAThread]
        static void Main() {
            if (CheckIfRunning())
                return;

            AddOrUpdateStartup();

            ApplicationConfiguration.Initialize();
            Application.Run(new TrayService());
        }

        static Boolean CheckIfRunning() {
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 1) {
                MessageBox.Show("This application is already running.", AppName);
                return true;
            }
            return false;
        }

        static void AddOrUpdateStartup() {
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
    }
}