using OneQuick.Config;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace OneQuick.Controls
{
    public partial class ScreenBorderButtonControl : ToggleButton, INotifyPropertyChanged
    {
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

        public ScreenBorder ScreenBorder
        {
            get => (ScreenBorder)GetValue(ScreenBorderProperty);
            set => SetValue(ScreenBorderProperty, value);
        }

        public static void OnScreenBorderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScreenBorderButtonControl screenBorderButtonControl = sender as ScreenBorderButtonControl;
            if (e.OldValue != null)
            {
                ((ScreenBorder)e.OldValue).PropertyChanged -= screenBorderButtonControl.ScreenBorder_PropertyChanged;
            }
            if (e.NewValue != null)
            {
                ((ScreenBorder)e.NewValue).PropertyChanged += screenBorderButtonControl.ScreenBorder_PropertyChanged;
            }
            screenBorderButtonControl.ScreenBorder_PropertyChanged(screenBorderButtonControl, new PropertyChangedEventArgs("ScreenBorder"));
        }

        private void ScreenBorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ScreenBorder");
            OnPropertyChanged("IsNonMove");
            OnPropertyChanged("IsWheel");
            OnPropertyChanged("IsWheelDown");
            OnPropertyChanged("IsWheelUp");
            OnPropertyChanged("IsWheelClick");
        }

        public bool IsNonMove => ScreenBorder != null && ScreenBorder.NonMove != null;

        public bool IsWheel => ScreenBorder != null && ScreenBorder.Wheel != WheelOperation.None && ScreenBorder.Wheel != WheelOperation.Custom;

        public bool IsWheelDown => ScreenBorder != null && ScreenBorder.Wheel == WheelOperation.Custom && ScreenBorder.WheelDown != null;

        public bool IsWheelUp => ScreenBorder != null && ScreenBorder.Wheel == WheelOperation.Custom && ScreenBorder.WheelUp != null;

        public bool IsWheelClick => ScreenBorder != null && ScreenBorder.WheelClick != null;

        public ScreenBorderButtonControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ScreenBorderProperty = DependencyProperty.Register("ScreenBorder", typeof(ScreenBorder), typeof(ScreenBorderButtonControl), new FrameworkPropertyMetadata(null)
        {
            BindsTwoWayByDefault = true,
            PropertyChangedCallback = new PropertyChangedCallback(OnScreenBorderPropertyChanged)
        });
    }
}
