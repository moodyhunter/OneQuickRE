using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OneQuick.Notification
{
    public class TrayIcon
    {
        public event Action<object, MouseButtons> MouseDown;

        public event Action<object, MouseButtons> MouseClick;

        public event Action<object, EventArgs> DoubleClick;

        public event Action<object, EventArgs> BalloonTipClicked;

        public string Text
        {
            get => _trayicon.Text;
            set => _trayicon.Text = value;
        }

        public Icon Icon
        {
            get => _trayicon.Icon;
            set => _trayicon.Icon = value;
        }

        public bool Visible
        {
            get => _trayicon.Visible;
            set => _trayicon.Visible = value;
        }

        public TrayIcon()
        {
            _trayicon = new NotifyIcon();
            _trayicon.MouseClick += _trayicon_MouseClick;
            _trayicon.DoubleClick += _trayicon_DoubleClick;
            _trayicon.BalloonTipClicked += _trayicon_BalloonTipClicked;
            _trayicon.MouseDown += _trayicon_MouseDown;
        }

        private void _trayicon_MouseDown(object sender, MouseEventArgs e)
        {
            Action<object, MouseButtons> mouseDown = MouseDown;
            if (mouseDown == null)
            {
                return;
            }
            mouseDown(this, (MouseButtons)e.Button);
        }

        private MenuItem Transform(TrayMenuItemBase trayMenuItem)
        {
            if (trayMenuItem == null)
            {
                return null;
            }
            if (trayMenuItem is TrayMenuSeperator)
            {
                return new MenuItem("-");
            }
            TrayMenuItem x;
            if ((x = trayMenuItem as TrayMenuItem) != null)
            {
                MenuItem menuItem = new MenuItem(x.Text, delegate (object o, EventArgs e)
                {
                    x.OnClicked();
                })
                {
                    Checked = x.Checked,
                    DefaultItem = x.Default
                };
                if (x.SubItems != null)
                {
                    foreach (TrayMenuItemBase trayMenuItem2 in x.SubItems)
                    {
                        menuItem.MenuItems.Add(Transform(trayMenuItem2));
                    }
                }
                return menuItem;
            }
            return null;
        }

        public void SetContextMenu(List<TrayMenuItemBase> list)
        {
            ContextMenu contextMenu = new ContextMenu();
            foreach (TrayMenuItemBase trayMenuItem in list)
            {
                contextMenu.MenuItems.Add(Transform(trayMenuItem));
            }
            _trayicon.ContextMenu = contextMenu;
        }

        public void Ballon(string Title, string Text, int milliseconds)
        {
            _trayicon.ShowBalloonTip(milliseconds, Title, Text, ToolTipIcon.None);
        }

        public void Hide()
        {
            _trayicon.Visible = false;
        }

        public void Show()
        {
            _trayicon.Visible = true;
        }

        private void _trayicon_MouseClick(object sender, MouseEventArgs e)
        {
            Action<object, MouseButtons> mouseClick = MouseClick;
            if (mouseClick == null)
            {
                return;
            }
            mouseClick(this, (MouseButtons)e.Button);
        }

        private void _trayicon_DoubleClick(object sender, EventArgs e)
        {
            Action<object, EventArgs> doubleClick = DoubleClick;
            if (doubleClick == null)
            {
                return;
            }
            doubleClick(this, e);
        }

        private void _trayicon_BalloonTipClicked(object sender, EventArgs e)
        {
            Action<object, EventArgs> balloonTipClicked = BalloonTipClicked;
            if (balloonTipClicked == null)
            {
                return;
            }
            balloonTipClicked(this, e);
        }

        private readonly NotifyIcon _trayicon;
    }

    public class TrayMenuItemBase
    {
    }

    internal class TrayMenuSeperator : TrayMenuItemBase
    {
    }

}
