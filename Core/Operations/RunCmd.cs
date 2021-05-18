using OneQuick.Notification;
using OneQuick.SysX;
using System;
using System.Web;
using System.Windows;

namespace OneQuick.Core.Operations
{
    public class RunCmd : Operation
    {
        public string Command
        {
            get => _command;
            set
            {
                _command = value;
                OnPropertyChanged("Command");
            }
        }

        public bool ShowCmdWindow
        {
            get => _showCmdWindow;
            set
            {
                _showCmdWindow = value;
                OnPropertyChanged("ShowCmdWindow");
            }
        }

        public string ReplaceClipboardTo
        {
            get => _replaceClipboardTo;
            set
            {
                _replaceClipboardTo = value;
                OnPropertyChanged("ReplaceClipboardTo");
            }
        }

        public RunCmd()
        {
            Mode = SyncMode.Task;
            ShowCmdWindow = true;
            ReplaceClipboardTo = "";
        }

        public RunCmd(string cmd) : this()
        {
            Command = cmd;
        }

        protected override bool EmptyParameter()
        {
            return Command == null || Command == "";
        }

        protected override void BeforeInvoke()
        {
            if (!string.IsNullOrEmpty(ReplaceClipboardTo) && (Mode == SyncMode.Task || Mode == SyncMode.Thread))
            {
                throw new Exception(string.Format("{0}, ReplaceClipboardTo == {1} but Mode == {2}.", this, ReplaceClipboardTo, Mode));
            }
        }

        protected override void _invoke()
        {
            string text;
            if (ReplaceClipboardTo == "")
            {
                text = Command;
            }
            else
            {
                string text2 = Clipboard.GetText();
                if (Cmd.IsUrl(Command))
                {
                    text2 = HttpUtility.UrlPathEncode(text2);
                }
                text = Command.Replace(ReplaceClipboardTo, text2);
            }
            try
            {
                Cmd.RunSmart(text, ShowCmdWindow);
            }
            catch (Exception ex)
            {
                Log.Error(ex, new string[]
                {
                    "[Run Command]" + text
                });
                Notify.PopNewToast(ex.Message + "\n" + text, null);
            }
        }

        public override string ToString()
        {
            return string.Format(">{0}", Command);
        }

        private string _command;

        private bool _showCmdWindow;

        private string _replaceClipboardTo;
    }
}
