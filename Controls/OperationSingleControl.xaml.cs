using OneQuick.Core;
using OneQuick.Core.Operations;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OneQuick.Controls
{
    public partial class OperationSingleControl : UserControl, INotifyPropertyChanged
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

        public K OperationKey
        {
            get
            {
                if (Operation is SendKey)
                {
                    return ((SendKey)Operation).Key;
                }
                return K.None;
            }
            set
            {
                if (Operation is SendKey)
                {
                    ((SendKey)Operation).Key = value;
                    return;
                }
                Operation = new SendKey(value);
            }
        }

        public string OperationText
        {
            get
            {
                if (Operation is SendText)
                {
                    return ((SendText)Operation).Text;
                }
                return "";
            }
            set
            {
                if (Operation is SendText)
                {
                    ((SendText)Operation).Text = value;
                    return;
                }
                Operation = new SendText(value, 0);
            }
        }

        public string OperationCmd
        {
            get
            {
                if (Operation is RunCmd)
                {
                    return ((RunCmd)Operation).Command;
                }
                return "";
            }
            set
            {
                if (Operation is RunCmd)
                {
                    ((RunCmd)Operation).Command = value;
                    return;
                }
                Operation = new RunCmd(value);
            }
        }

        public Operation Operation
        {
            get => (Operation)GetValue(OperationProperty);
            set => SetValue(OperationProperty, value);
        }

        private static void OnOperationChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OperationSingleControl operationSingleControl = d as OperationSingleControl;
            operationSingleControl.OnOperationPropertyChanged(operationSingleControl, e);
        }

        private void OnOperationPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PassOperationPropertyChanged)
            {
                return;
            }
            Operation operation = e.NewValue as Operation;
            if (operation != null)
            {
                BuildinOperation buildinOperation;
                if ((buildinOperation = operation as BuildinOperation) == null)
                {
                    SendKey sendKey;
                    if ((sendKey = operation as SendKey) == null)
                    {
                        if (!(operation is SendText))
                        {
                            if (operation is RunCmd)
                            {
                                ComboBox.SelectedItem = RunCmdItem;
                            }
                            else
                            {
                                ComboBox.SelectedItem = NoneItem;
                            }
                        }
                        else
                        {
                            ComboBox.SelectedItem = SendTextItem;
                        }
                    }
                    else
                    {
                        switch (sendKey.Key)
                        {
                            case K.VolumeMute:
                                ComboBox.SelectedItem = VolumeMuteItem;
                                goto IL_232;
                            case K.VolumeDown:
                                ComboBox.SelectedItem = VolumeDownItem;
                                goto IL_232;
                            case K.VolumeUp:
                                ComboBox.SelectedItem = VolumeUpItem;
                                goto IL_232;
                            case K.MediaNextTrack:
                                ComboBox.SelectedItem = MediaNextItem;
                                goto IL_232;
                            case K.MediaPreviousTrack:
                                ComboBox.SelectedItem = MediaPrevItem;
                                goto IL_232;
                            case K.MediaPlayPause:
                                ComboBox.SelectedItem = MediaPlayPauseItem;
                                goto IL_232;
                        }
                        ComboBox.SelectedItem = SendKeyItem;
                    }
                }
                else
                {
                    switch (buildinOperation.OperationEnum)
                    {
                        case BuildinOperationEnum.Block:
                            ComboBox.SelectedItem = BlockItem;
                            break;
                        case BuildinOperationEnum.ToggleTopmost:
                            ComboBox.SelectedItem = TopmostItem;
                            break;
                        case BuildinOperationEnum.OpacityDown:
                            ComboBox.SelectedItem = OpacityDownItem;
                            break;
                        case BuildinOperationEnum.OpacityUp:
                            ComboBox.SelectedItem = OpacityUpItem;
                            break;
                        case BuildinOperationEnum.MonitorOff:
                            ComboBox.SelectedItem = MonitorOffItem;
                            break;
                        case BuildinOperationEnum.Suspend:
                            ComboBox.SelectedItem = SuspendedItem;
                            break;
                        case BuildinOperationEnum.Hibernate:
                            ComboBox.SelectedItem = HibernateItem;
                            break;
                    }
                }
            }
            else
            {
                ComboBox.SelectedItem = NoneItem;
            }
        IL_232:
            OnPropertyChanged("Operation");
            OnPropertyChanged("OperationKey");
            OnPropertyChanged("OperationText");
            OnPropertyChanged("OperationCmd");
        }

        public OperationSingleControl()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PassComboBoxSelectionChanged)
            {
                return;
            }
            string text = (ComboBox.SelectedItem as ComboBoxItem).Tag as string;
            uint num = Internal.ComputeStringHash(text);
            if (num <= 1751276452u)
            {
                if (num <= 589956806u)
                {
                    if (num <= 270379342u)
                    {
                        if (num != 54146166u)
                        {
                            if (num == 270379342u)
                            {
                                if (text == "OpacityDown")
                                {
                                    Operation = new BuildinOperation(BuildinOperationEnum.OpacityDown);
                                }
                            }
                        }
                        else if (text == "SendKey")
                        {
                            if (!(Operation is SendKey))
                            {
                                Operation = new SendKey();
                            }
                        }
                    }
                    else if (num != 347702362u)
                    {
                        if (num == 589956806u)
                        {
                            if (text == "VolumeMute")
                            {
                                OperationKey = K.VolumeMute;
                            }
                        }
                    }
                    else if (text == "MediaNext")
                    {
                        OperationKey = K.MediaNextTrack;
                    }
                }
                else if (num <= 970295310u)
                {
                    if (num != 810547195u)
                    {
                        if (num == 970295310u)
                        {
                            if (text == "MediaPrev")
                            {
                                OperationKey = K.MediaPreviousTrack;
                            }
                        }
                    }
                    else if (text == "None")
                    {
                        Operation = null;
                    }
                }
                else if (num != 1099992830u)
                {
                    if (num == 1751276452u)
                    {
                        if (text == "PowerHibernate")
                        {
                            Operation = new BuildinOperation(BuildinOperationEnum.Hibernate);
                        }
                    }
                }
                else if (text == "MonitorOff")
                {
                    Operation = new BuildinOperation(BuildinOperationEnum.MonitorOff);
                }
            }
            else if (num <= 2458933433u)
            {
                if (num <= 1835966363u)
                {
                    if (num != 1834328282u)
                    {
                        if (num == 1835966363u)
                        {
                            if (text == "OpacityUp")
                            {
                                Operation = new BuildinOperation(BuildinOperationEnum.OpacityUp);
                            }
                        }
                    }
                    else if (text == "VolumeUp")
                    {
                        OperationKey = K.VolumeUp;
                    }
                }
                else if (num != 2087139778u)
                {
                    if (num == 2458933433u)
                    {
                        if (text == "MediaPlayPause")
                        {
                            OperationKey = K.MediaPlayPause;
                        }
                    }
                }
                else if (text == "RunCmd")
                {
                    if (!(Operation is RunCmd))
                    {
                        Operation = new RunCmd();
                    }
                }
            }
            else if (num <= 3171368962u)
            {
                if (num != 2623005851u)
                {
                    if (num == 3171368962u)
                    {
                        if (text == "Block")
                        {
                            Operation = new BuildinOperation(BuildinOperationEnum.Block);
                        }
                    }
                }
                else if (text == "VolumeDown")
                {
                    OperationKey = K.VolumeDown;
                }
            }
            else if (num != 3682733720u)
            {
                if (num != 3729219659u)
                {
                    if (num == 3937500851u)
                    {
                        if (text == "ToggleTopmost")
                        {
                            Operation = new BuildinOperation(BuildinOperationEnum.ToggleTopmost);
                        }
                    }
                }
                else if (text == "PowerSuspended")
                {
                    Operation = new BuildinOperation(BuildinOperationEnum.Suspend);
                }
            }
            else if (text == "SendText")
            {
                if (!(Operation is SendText))
                {
                    Operation = new SendText();
                }
            }
            PassComboBoxSelectionChanged = false;
            PassOperationPropertyChanged = false;
        }

        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register("Operation", typeof(Operation), typeof(OperationSingleControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        {
            PropertyChangedCallback = new PropertyChangedCallback(OnOperationChangedStatic)
        });

        private bool PassOperationPropertyChanged;

        private bool PassComboBoxSelectionChanged;
    }
}
