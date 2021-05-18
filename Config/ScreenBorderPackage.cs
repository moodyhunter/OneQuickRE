using OneQuick.Core.Conditions;
using System.Collections.Generic;
using System.ComponentModel;

namespace OneQuick.Config
{
    public class ScreenBorderPackage : INotifyPropertyChanged
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

        public int BorderPixel { get; set; }

        public ScreenBorder LT
        {
            get => _lt;
            set
            {
                if (_lt != null)
                {
                    _lt.PropertyChanged -= _lt_PropertyChanged;
                }
                _lt = value;
                _lt.PropertyChanged += _lt_PropertyChanged;
                _lt.Position = PositionEnum.TopLeft;
                OnPropertyChanged("LT");
            }
        }

        private void _lt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("LT");
        }

        public ScreenBorder T
        {
            get => _t;
            set
            {
                if (_t != null)
                {
                    _t.PropertyChanged -= _t_PropertyChanged;
                }
                _t = value;
                _t.PropertyChanged += _t_PropertyChanged;
                _t.Position = PositionEnum.TopCenter;
                OnPropertyChanged("T");
            }
        }

        private void _t_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("T");
        }

        public ScreenBorder RT
        {
            get => _rt;
            set
            {
                if (_rt != null)
                {
                    _rt.PropertyChanged -= _rt_PropertyChanged;
                }
                _rt = value;
                _rt.PropertyChanged += _rt_PropertyChanged;
                _rt.Position = PositionEnum.TopRight;
                OnPropertyChanged("RT");
            }
        }

        private void _rt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("RT");
        }

        public ScreenBorder R
        {
            get => _r;
            set
            {
                if (_r != null)
                {
                    _r.PropertyChanged -= _r_PropertyChanged;
                }
                _r = value;
                _r.PropertyChanged += _r_PropertyChanged;
                _r.Position = PositionEnum.CenterRight;
                OnPropertyChanged("R");
            }
        }

        private void _r_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("R");
        }

        public ScreenBorder RB
        {
            get => _rb;
            set
            {
                if (_rb != null)
                {
                    _rb.PropertyChanged -= _rb_PropertyChanged;
                }
                _rb = value;
                _rb.PropertyChanged += _rb_PropertyChanged;
                _rb.Position = PositionEnum.BottomRight;
                OnPropertyChanged("RB");
            }
        }

        private void _rb_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("RB");
        }

        public ScreenBorder B
        {
            get => _b;
            set
            {
                if (_b != null)
                {
                    _b.PropertyChanged -= _b_PropertyChanged;
                }
                _b = value;
                _b.PropertyChanged += _b_PropertyChanged;
                _b.Position = PositionEnum.BottomCenter;
                OnPropertyChanged("B");
            }
        }

        private void _b_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("B");
        }

        public ScreenBorder LB
        {
            get => _lb;
            set
            {
                if (_lb != null)
                {
                    _lb.PropertyChanged -= _lb_PropertyChanged;
                }
                _lb = value;
                _lb.PropertyChanged += _lb_PropertyChanged;
                _lb.Position = PositionEnum.BottomLeft;
                OnPropertyChanged("LB");
            }
        }

        private void _lb_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("LB");
        }

        public ScreenBorder L
        {
            get => _l;
            set
            {
                if (_l != null)
                {
                    _l.PropertyChanged -= _l_PropertyChanged;
                }
                _l = value;
                _l.PropertyChanged += _l_PropertyChanged;
                _l.Position = PositionEnum.CenterLeft;
                OnPropertyChanged("L");
            }
        }

        private void _l_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("L");
        }

        public IEnumerable<ScreenBorder> IterAll()
        {
            yield return LT;
            yield return T;
            yield return RT;
            yield return R;
            yield return RB;
            yield return B;
            yield return LB;
            yield return L;
            yield break;
        }

        public ScreenBorderPackage()
        {
            BorderPixel = 8;
            LT = new ScreenBorder();
            T = new ScreenBorder();
            RT = new ScreenBorder();
            R = new ScreenBorder();
            RB = new ScreenBorder();
            B = new ScreenBorder();
            LB = new ScreenBorder();
            L = new ScreenBorder();
        }

        private ScreenBorder _lt;

        private ScreenBorder _t;

        private ScreenBorder _rt;

        private ScreenBorder _r;

        private ScreenBorder _rb;

        private ScreenBorder _b;

        private ScreenBorder _lb;

        private ScreenBorder _l;
    }
}
