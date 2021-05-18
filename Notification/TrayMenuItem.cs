using System;
using System.Collections.Generic;

namespace OneQuick.Notification
{
    public class TrayMenuItem : TrayMenuItemBase
    {
        public string Text { get; set; }

        public bool Checked { get; set; }

        public bool Default { get; set; }

        public event Action Click;

        public void OnClicked()
        {
            Action click = Click;
            if (click == null)
            {
                return;
            }
            click();
        }

        public TrayMenuItem(string text)
        {
            Text = text;
        }

        public TrayMenuItem(string text, Action action)
        {
            Text = text;
            Click += action;
        }

        public List<TrayMenuItemBase> SubItems;
    }
}
