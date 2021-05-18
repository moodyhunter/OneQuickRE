using OneQuick.Core;
using OneQuick.Notification;
using OneQuick.WindowsEvents;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OneQuick.Controls
{
    public partial class KControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public HorizontalAlignment KTextAlignment
        {
            get => (HorizontalAlignment)GetValue(KTextAlignmentProperty);
            set => SetValue(KTextAlignmentProperty, value);
        }

        public bool SingleModKey
        {
            get => (bool)GetValue(SingleModKeyProperty);
            set => SetValue(SingleModKeyProperty, value);
        }

        public bool EnterBackspacePlanChecked
        {
            get => ConfirmClear == ConfirmClearPlan.EnterBackspace;
            set => ConfirmClear = ConfirmClearPlan.EnterBackspace;
        }

        public bool F1F2PlanChecked
        {
            get => ConfirmClear == ConfirmClearPlan.F1F2;
            set => ConfirmClear = ConfirmClearPlan.F1F2;
        }

        public ConfirmClearPlan ConfirmClear
        {
            get => (ConfirmClearPlan)GetValue(ConfirmClearProperty);
            set => SetValue(ConfirmClearProperty, value);
        }

        private static void OnConfirmClearPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            KControl kcontrol = (KControl)sender;
            kcontrol.OnPropertyChanged("ConfirmClear");
            kcontrol.OnPropertyChanged("EnterBackspacePlanChecked");
            kcontrol.OnPropertyChanged("F1F2PlanChecked");
        }

        public Kwrapper Kwrapper
        {
            get => (Kwrapper)GetValue(KwrapperProperty);
            set => SetValue(KwrapperProperty, value);
        }

        private static void OnKwrapperPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            KControl kcontrol = (KControl)sender;
            Kwrapper kwrapper = kcontrol.Kwrapper;
            kcontrol.K = (kwrapper != null) ? kwrapper.KeyData : K.None;
            kcontrol.OnPropertyChanged("KString");
        }

        public K K
        {
            get => (K)GetValue(KProperty);
            set => SetValue(KProperty, value);
        }

        private static void OnKPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            KControl kcontrol = (KControl)sender;
            kcontrol.Kwrapper = new Kwrapper(kcontrol.K);
            if (kcontrol == null)
            {
                return;
            }
            kcontrol.OnPropertyChanged("KString");
        }

        public string EmptyTipString { get; set; } = "";

        public string KString
        {
            get
            {
                if (KListen != K.None)
                {
                    return KListen.FormatString();
                }
                string text = K.FormatString();
                if (text == "")
                {
                    return EmptyTipString;
                }
                return text;
            }
        }

        public KControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartListen();
        }

        private K KPressed { get; set; }

        private K KListen { get; set; }

        private void KeyEventProc(HotKeyEventArgs e, bool IsKeyDown)
        {
            e.Handled = true;
            bool flag = false;
            bool flag2 = false;
            if (ConfirmClear == ConfirmClearPlan.EnterBackspace)
            {
                flag = e.KeyCode == K.Return;
                flag2 = e.KeyCode == K.Back;
            }
            else if (ConfirmClear == ConfirmClearPlan.F1F2)
            {
                flag = e.KeyCode == K.F1;
                flag2 = e.KeyCode == K.F2;
            }
            if (flag || (KPressed == K.None && (e.KeyCode == K.LButton || e.KeyCode == K.RButton)))
            {
                StopListen();
                return;
            }
            if (flag2)
            {
                KListen = K.None;
                StopListen();
                return;
            }
            if (SingleModKey)
            {
                KListen = e.RawKeyCode;
                StopListen();
                return;
            }
            if (!IsKeyDown && e.KeyData.ModifiersKey() != K.None && e.KeyData == KPressed)
            {
                StopListen();
                return;
            }
            if (IsKeyDown)
            {
                KPressed |= e.KeyData;
            }
            else
            {
                KPressed &= ~e.KeyData;
            }
            KListen = KPressed;
            if (IsKeyDown && e.KeyCode != K.None)
            {
                StopListen();
            }
            OnPropertyChanged("KString");
        }

        private void EventsServer_KeyDown(HotKeyEventArgs e)
        {
            KeyEventProc(e, true);
        }

        private void EventsServer_KeyUp(HotKeyEventArgs e)
        {
            KeyEventProc(e, false);
        }

        private void StartListen()
        {
            if (!EventsServer.Enable)
            {
                Notify.ShowMsg("Windows hook off", "");
                return;
            }
            KListen = K;
            KPressed = K.None;
            OnPropertyChanged("KString");
            EntryServer.EnableCounter = false;
            EventsServer.HotkeyDown += EventsServer_KeyDown;
            EventsServer.HotkeyUp += EventsServer_KeyUp;
            Border.Background = Brushes.Red;
        }

        private void StopListen()
        {
            K = KListen;
            KListen = K.None;
            KPressed = K.None;
            OnPropertyChanged("KString");
            EntryServer.EnableCounter = true;
            EventsServer.HotkeyDown -= EventsServer_KeyDown;
            EventsServer.HotkeyUp -= EventsServer_KeyUp;
            Border.Background = Brushes.Black;
        }

        private void ClearKey(object sender, RoutedEventArgs e)
        {
            K = K.None;
        }

        public static readonly DependencyProperty KTextAlignmentProperty = DependencyProperty.Register("KTextAlignment", typeof(HorizontalAlignment), typeof(KControl), new PropertyMetadata(HorizontalAlignment.Center));

        public static readonly DependencyProperty SingleModKeyProperty = DependencyProperty.Register("SingleModKey", typeof(bool), typeof(KControl), new PropertyMetadata(false));

        public static readonly DependencyProperty ConfirmClearProperty = DependencyProperty.Register("ConfirmClear", typeof(ConfirmClearPlan), typeof(KControl), new FrameworkPropertyMetadata
        {
            DefaultValue = ConfirmClearPlan.EnterBackspace,
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnConfirmClearPropertyChanged)
        });

        public static readonly DependencyProperty KwrapperProperty = DependencyProperty.Register("Kwrapper", typeof(Kwrapper), typeof(KControl), new FrameworkPropertyMetadata
        {
            DefaultValue = null,
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnKwrapperPropertyChanged)
        });

        public static readonly DependencyProperty KProperty = DependencyProperty.Register("K", typeof(K), typeof(KControl), new FrameworkPropertyMetadata
        {
            DefaultValue = K.None,
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnKPropertyChanged)
        });

        public enum ConfirmClearPlan
        {
            EnterBackspace,
            F1F2
        }
    }
}
