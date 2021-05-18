using OneQuick.Core.Conditions;
using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OneQuick.Config
{
    public class ScreenBorder : ConfigEntry
    {
        [XmlIgnore]
        public PositionEnum Position
        {
            get => _position;
            internal set
            {
                _position = value;
                OnPropertyChanged("Position", TriggerUpdate.Now);
            }
        }

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged("Enable", TriggerUpdate.Now);
            }
        }

        public Operation NonMove
        {
            get => _nonMove;
            set
            {
                if (_nonMove != null)
                {
                    _nonMove.PropertyChanged -= _nonMove_PropertyChanged;
                }
                _nonMove = value;
                if (_nonMove != null)
                {
                    _nonMove.PropertyChanged += _nonMove_PropertyChanged;
                }
                OnPropertyChanged("NonMove", TriggerUpdate.Now);
            }
        }

        private void _nonMove_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("NonMove", TriggerUpdate.Now);
        }

        public WheelOperation Wheel
        {
            get => _wheel;
            set
            {
                _wheel = value;
                if (_wheel != WheelOperation.Custom)
                {
                    WheelDown = null;
                    WheelUp = null;
                }
                OnPropertyChanged("Wheel", TriggerUpdate.Now);
                OnPropertyChanged("WheelTranslate", TriggerUpdate.None);
            }
        }

        [XmlIgnore]
        public string WheelTranslate => Wheel.Translate();

        public Operation WheelDown
        {
            get => _wheelDown;
            set
            {
                if (_wheelDown != null)
                {
                    _wheelDown.PropertyChanged -= _wheelDown_PropertyChanged;
                }
                _wheelDown = value;
                if (_wheelDown != null)
                {
                    _wheelDown.PropertyChanged += _wheelDown_PropertyChanged;
                }
                OnPropertyChanged("WheelDown", TriggerUpdate.Now);
            }
        }

        private void _wheelDown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("WheelDown", TriggerUpdate.Now);
        }

        public Operation WheelUp
        {
            get => _wheelUp;
            set
            {
                if (_wheelUp != null)
                {
                    _wheelUp.PropertyChanged -= _wheelUp_PropertyChanged;
                }
                _wheelUp = value;
                if (_wheelUp != null)
                {
                    _wheelUp.PropertyChanged += _wheelUp_PropertyChanged;
                }
                OnPropertyChanged("WheelUp", TriggerUpdate.Now);
            }
        }

        private void _wheelUp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("WheelUp", TriggerUpdate.Now);
        }

        public Operation WheelClick
        {
            get => _wheelClick;
            set
            {
                if (_wheelClick != null)
                {
                    _wheelClick.PropertyChanged -= _wheelClick_PropertyChanged;
                }
                _wheelClick = value;
                if (_wheelClick != null)
                {
                    _wheelClick.PropertyChanged += _wheelClick_PropertyChanged;
                }
                OnPropertyChanged("WheelClick", TriggerUpdate.Now);
            }
        }

        private void _wheelClick_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("WheelClick", TriggerUpdate.Now);
        }

        public ScreenBorder()
        {
            Enable = true;
        }

        protected MouseScreenPosCodition GetPositionCondition()
        {
            return new MouseScreenPosCodition
            {
                Position = new PositionWrapper(Position)
            };
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            if (!Enable)
            {
                return null;
            }
            List<Trigger> list = new List<Trigger>();
            if (NonMove != null)
            {
                list.Add(new NonMoveTrigger(2000)
                {
                    Condition = GetPositionCondition(),
                    Operation = NonMove
                });
            }
            if (WheelClick != null)
            {
                list.Add(new HotkeyTrigger(K.MButton)
                {
                    Condition = GetPositionCondition(),
                    Operation = WheelClick
                });
            }
            if (Wheel != WheelOperation.None)
            {
                WheelOperation wheel = Wheel;
                Operation operation;
                Operation operation2;
                if (wheel <= WheelOperation.VirtualDesktop)
                {
                    if (wheel == WheelOperation.Volume)
                    {
                        operation = new SendKey(K.VolumeDown);
                        operation2 = new SendKey(K.VolumeUp);
                        goto IL_17D;
                    }
                    if (wheel == WheelOperation.Media)
                    {
                        operation = new SendKey(K.MediaNextTrack);
                        operation2 = new SendKey(K.MediaPreviousTrack);
                        goto IL_17D;
                    }
                    switch (wheel)
                    {
                        case WheelOperation.Tab:
                            operation = new SendKey((K)131081);
                            operation2 = new SendKey((K)196617);
                            goto IL_17D;
                        case WheelOperation.Window:
                            operation = new SendKey((K)262171);
                            operation2 = new SendKey((K)327707);
                            goto IL_17D;
                        case WheelOperation.VirtualDesktop:
                            operation = new SendKey((K)655399);
                            operation2 = new SendKey((K)655397);
                            goto IL_17D;
                    }
                }
                else
                {
                    if (wheel == WheelOperation.PageDownUp)
                    {
                        operation = new SendKey(K.PageDown);
                        operation2 = new SendKey(K.PageUp);
                        goto IL_17D;
                    }
                    if (wheel == WheelOperation.HomeEnd)
                    {
                        operation = new SendKey(K.End);
                        operation2 = new SendKey(K.Home);
                        goto IL_17D;
                    }
                    if (wheel == WheelOperation.Custom)
                    {
                        operation = WheelDown;
                        operation2 = WheelUp;
                        goto IL_17D;
                    }
                }
                throw new Exception();
            IL_17D:
                if (operation != null)
                {
                    list.Add(new HotkeyTrigger(K.WheelDown)
                    {
                        Condition = GetPositionCondition(),
                        Operation = operation
                    });
                }
                if (operation2 != null)
                {
                    list.Add(new HotkeyTrigger(K.WheelUp)
                    {
                        Condition = GetPositionCondition(),
                        Operation = operation2
                    });
                }
            }
            foreach (Trigger trigger in list)
            {
                trigger.TriggerType = TriggerType.ScreenBorder;
            }
            return list;
        }

        public override string ToString()
        {
            string text = Position.ToString();
            if (NonMove != null)
            {
                text = text + "\nHot Corners: " + NonMove.ToString();
            }
            if (Wheel != WheelOperation.None)
            {
                text = text + "\nWheel: " + Wheel.ToString();
            }
            if (WheelDown != null)
            {
                text = text + "\nWheelDown: " + WheelDown.ToString();
            }
            if (WheelUp != null)
            {
                text = text + "\nWheelUp: " + WheelUp.ToString();
            }
            if (WheelClick != null)
            {
                text = text + "\nWheelClick: " + WheelClick.ToString();
            }
            return text;
        }

        private PositionEnum _position;

        private bool _enable;

        private Operation _nonMove;

        private WheelOperation _wheel;

        private Operation _wheelDown;

        private Operation _wheelUp;

        private Operation _wheelClick;
    }
}
