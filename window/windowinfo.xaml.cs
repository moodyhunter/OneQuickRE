using OneQuick.SysX;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace OneQuick
{
    public partial class WindowInfo : Window, IComponentConnector
    {
        public static void ShowInfo(IntPtr? hWnd = null)
        {
            new WindowInfo(hWnd ?? Win.GetForegroundWindow()).Show();
        }

        public IntPtr WinHandle { get; private set; }

        public bool IsTopmost
        {
            get => Win.GetTopmostStatus(new IntPtr?(WinHandle));
            set => Win.ToggleTopmost(new IntPtr?(WinHandle), new bool?(value));
        }

        public string WinTitle => Win.GetWindowTitle(new IntPtr?(WinHandle));

        public string WinPath => Win.GetWindowProcFileName(new IntPtr?(WinHandle));

        public double WinOpacity
        {
            get => Win.GetOpacity(new IntPtr?(WinHandle));
            set => Win.SetOpacity(value, new IntPtr?(WinHandle));
        }

        private WindowInfo(IntPtr ptr)
        {
            WinHandle = ptr;
            InitializeComponent();
            Title = "Window Info: " + WinTitle;
            RoutedCommand routedCommand = new RoutedCommand();
            routedCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(routedCommand, delegate (object o, ExecutedRoutedEventArgs e)
            {
                Close();
            }));
        }

        private void TextBox_WinPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cmd.Explorer(WinPath);
        }
    }
}
