using OneQuick.Config;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OneQuick.Controls
{
    public partial class ScreenBorderDetailControl : UserControl, INotifyPropertyChanged
    {
        public ScreenBorder ScreenBorder
        {
            get => (ScreenBorder)GetValue(ScreenBorderProperty);
            set => SetValue(ScreenBorderProperty, value);
        }

        public static void OnScreenBorderPropertyChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScreenBorderDetailControl screenBorderDetailControl = (ScreenBorderDetailControl)sender;
            screenBorderDetailControl.OnScreenBorderChanged(screenBorderDetailControl, e);
            if (e.OldValue != null)
            {
                ((ScreenBorder)e.OldValue).PropertyChanged -= screenBorderDetailControl.ScreenBorder_PropertyChanged;
            }
            if (e.NewValue != null)
            {
                ((ScreenBorder)e.NewValue).PropertyChanged += screenBorderDetailControl.ScreenBorder_PropertyChanged;
            }
        }

        internal void OnScreenBorderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WheelOperation wheel = (e.NewValue as ScreenBorder).Wheel;
            ComboBox wheelComboBox = WheelComboBox;
            foreach (object obj in (wheelComboBox != null) ? wheelComboBox.Items : null)
            {
                ComboBoxItem comboBoxItem = (ComboBoxItem)obj;
                if (wheel == WheelOperationExt.Parse(comboBoxItem.Tag))
                {
                    comboBoxItem.IsSelected = true;
                }
            }
            ScreenBorder_PropertyChanged(sender, null);
        }

        internal void ScreenBorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ScreenBorder");
            OnPropertyChanged("IsWheelCustom");
            OnPropertyChanged("IsWheelCustomVisibility");
        }

        public bool IsWheelCustom => ScreenBorder.Wheel == WheelOperation.Custom;

        public Visibility IsWheelCustomVisibility
        {
            get
            {
                if (!IsWheelCustom)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public ScreenBorderDetailControl()
        {
            InitializeComponent();
        }

        private void WheelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WheelOperation wheel = WheelOperationExt.Parse((WheelComboBox.SelectedItem as ComboBoxItem).Tag);
            ScreenBorder.Wheel = wheel;
            lastWheelOperation = wheel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public static readonly DependencyProperty ScreenBorderProperty = DependencyProperty.Register("ScreenBorder", typeof(ScreenBorder), typeof(ScreenBorderDetailControl), new FrameworkPropertyMetadata(null)
        {
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnScreenBorderPropertyChangedStatic)
        });

        private WheelOperation lastWheelOperation;
    }
}
