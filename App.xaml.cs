using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OneQuick.Notification;
using OneQuick.WindowsEvents;

namespace OneQuick
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            window = new MainWindow();
            MainWindow = window;
            window.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventsServer.Enable = false;
            Exception ex = (Exception)e.ExceptionObject;
            string text;
            if (ex.Data.Contains("PROC_MSG"))
            {
                text = (string)ex.Data["PROC_MSG"];
            }
            else
            {
                text = Helper.ProcessFatalUnhandledException(ex);
            }
            G.MainWindow.TrayIcon.Hide();
            G.MainWindow.Visibility = Visibility.Collapsed;
            Notify.ShowMsg(text.Substring(0, 1000) + "\r\n......", "CRASHED");
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string value = Helper.ProcessFatalUnhandledException(e.Exception);
            e.Exception.Data.Add("PROC_MSG", value);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Helper.ProcessFatalUnhandledException(e.Exception);
        }

        private MainWindow window;
    }
}
