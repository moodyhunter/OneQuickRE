using OneQuick.Core.Triggers;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OneQuick.Controls
{
    public partial class HotkeyControl : UserControl, INotifyPropertyChanged
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

        public bool SingleHotkey => !MultipleHotkey;

        public bool MultipleHotkey
        {
            get => (bool)GetValue(MultipleHotkeyProperty);
            set => SetValue(MultipleHotkeyProperty, value);
        }

        public int MaxHotkeyNum
        {
            get => (int)GetValue(MaxHotkeyNumProperty);
            set => SetValue(MaxHotkeyNumProperty, value);
        }

        public HorizontalAlignment TextAlignment
        {
            get => (HorizontalAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public string DefaultEmptyText
        {
            get => (string)GetValue(DefaultEmptyTextProperty);
            set => SetValue(DefaultEmptyTextProperty, value);
        }

        public HotkeyTrigger HotkeyTrigger
        {
            get => (HotkeyTrigger)GetValue(HotkeyTriggerProperty);
            set => SetValue(HotkeyTriggerProperty, value);
        }

        private static void OnHotkeyTriggerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HotkeyControl hotkeyControl = (HotkeyControl)sender;
            if (hotkeyControl == null)
            {
                return;
            }
            hotkeyControl.OnPropertyChanged("Text");
        }

        public Visibility MultipleTipVisibility { get; private set; } = Visibility.Collapsed;

        public HotkeyControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MultipleHotkeyProperty = DependencyProperty.Register("MultipleHotkey", typeof(bool), typeof(HotkeyControl), new PropertyMetadata(false));

        public static readonly DependencyProperty MaxHotkeyNumProperty = DependencyProperty.Register("MaxHotkeyNum", typeof(int), typeof(HotkeyControl), new PropertyMetadata(3));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(HorizontalAlignment), typeof(HotkeyControl), new PropertyMetadata(HorizontalAlignment.Center));

        public static readonly DependencyProperty DefaultEmptyTextProperty = DependencyProperty.Register("DefaultEmptyText", typeof(string), typeof(HotkeyControl), new PropertyMetadata(""));

        public static readonly DependencyProperty HotkeyTriggerProperty = DependencyProperty.Register("HotkeyTrigger", typeof(HotkeyTrigger), typeof(HotkeyControl), new FrameworkPropertyMetadata
        {
            DefaultValue = null,
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnHotkeyTriggerPropertyChanged)
        });
    }
}
