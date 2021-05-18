using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OneQuick.Core.Conditions
{
    public class PositionWrapper : INotifyPropertyChanged
    {
        [XmlIgnore]
        public PositionEnum Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPositionChanged();
            }
        }

        public int PositionCode
        {
            get => (int)Position;
            set
            {
                Position = (PositionEnum)value;
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool TopCenter
        {
            get => Position.Include(PositionEnum.TopCenter);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.TopCenter;
                }
                else
                {
                    Position &= (PositionEnum)(-3);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool BottomCenter
        {
            get => Position.Include(PositionEnum.BottomCenter);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.BottomCenter;
                }
                else
                {
                    Position &= (PositionEnum)(-33);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool CenterLeft
        {
            get => Position.Include(PositionEnum.CenterLeft);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.CenterLeft;
                }
                else
                {
                    Position &= (PositionEnum)(-129);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool CenterRight
        {
            get => Position.Include(PositionEnum.CenterRight);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.CenterRight;
                }
                else
                {
                    Position &= (PositionEnum)(-9);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool TopLeft
        {
            get => Position.Include(PositionEnum.TopLeft);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.TopLeft;
                }
                else
                {
                    Position &= (PositionEnum)(-2);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool TopRight
        {
            get => Position.Include(PositionEnum.TopRight);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.TopRight;
                }
                else
                {
                    Position &= (PositionEnum)(-5);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool BottomLeft
        {
            get => Position.Include(PositionEnum.BottomLeft);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.BottomLeft;
                }
                else
                {
                    Position &= (PositionEnum)(-65);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool BottomRight
        {
            get => Position.Include(PositionEnum.BottomRight);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.BottomRight;
                }
                else
                {
                    Position &= (PositionEnum)(-17);
                }
                OnPositionChanged();
            }
        }

        [XmlIgnore]
        public bool Center
        {
            get => Position.Include(PositionEnum.Center);
            set
            {
                if (value)
                {
                    Position |= PositionEnum.Center;
                }
                else
                {
                    Position &= (PositionEnum)(-257);
                }
                OnPositionChanged();
            }
        }

        public PositionWrapper()
        {
            Position = PositionEnum.None;
        }

        public PositionWrapper(PositionEnum pos)
        {
            Position = pos;
        }

        public override string ToString()
        {
            int num = (int)Position;
            if (num == 0)
            {
                return "Nowhere";
            }
            if (num == 511)
            {
                return "Anywhere";
            }
            if (num == 255)
            {
                return "Border";
            }
            string text = "";
            List<int> list = new List<int>();
            list.Add(7);
            list.Add(193);
            list.Add(28);
            list.Add(112);
            int num2 = 0;
            foreach (int num3 in list)
            {
                if ((num & num3) == num3)
                {
                    string str = text;
                    PositionEnum positionEnum = (PositionEnum)num3;
                    text = str + positionEnum.ToString();
                    text += ", ";
                    num2 |= num3;
                }
            }
            num &= ~num2;
            int num4 = 1;
            for (int i = 0; i < 9; i++)
            {
                int num5 = num4 << i;
                if ((num & num5) == num5)
                {
                    string str2 = text;
                    PositionEnum positionEnum = (PositionEnum)num5;
                    text = str2 + positionEnum.ToString();
                    text += ", ";
                }
            }
            if (text.Length > 0)
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

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

        private void OnPositionChanged()
        {
            OnPropertyChanged("Position");
            OnPropertyChanged("PositionCode");
            OnPropertyChanged("TopCenter");
            OnPropertyChanged("BottomCenter");
            OnPropertyChanged("CenterLeft");
            OnPropertyChanged("CenterRight");
            OnPropertyChanged("TopLeft");
            OnPropertyChanged("TopRight");
            OnPropertyChanged("BottomLeft");
            OnPropertyChanged("BottomRight");
            OnPropertyChanged("Center");
        }

        private PositionEnum _position;
    }
}
