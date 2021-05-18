using System.Threading.Tasks;
using System.Windows;

namespace OneQuick.Notification
{
    internal static class Notify
    {
        public static void ShowMsg(string Msg, string Cap = "")
        {
            if (Cap == "")
            {
                Cap = "OneQuick";
            }
            MessageBox.Show(Msg, Cap);
        }

        public static bool AskYesNo(string Msg, string Cap = "")
        {
            if (Cap == "")
            {
                Cap = "OneQuick";
            }
            return MessageBox.Show(Msg, Cap, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public static bool AskOverwrite(string Cap = "")
        {
            return AskYesNo("AskOverWriteFile", "");
        }

        public static void PopNewToast(string Text, string Title = null)
        {
            if (Title == null)
            {
                Title = "OneQuick";
            }
            if (G.STORE)
            {
                Task.Run(delegate ()
                {
                    ClearToast();
                    //MyNotificationActivator.Pop(Text);
                });
                return;
            }
            G.TrayIcon.Ballon(Title, Text, 5000);
        }

        public static void ClearToast()
        {
            if (G.STORE)
            {
                //MyNotificationActivator.Clear();
            }
        }
    }
}
