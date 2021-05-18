using OneQuick.Notification;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OneQuick.Core.Operations
{
    [XmlInclude(typeof(OperationList))]
    [XmlInclude(typeof(PopMsg))]
    [XmlInclude(typeof(RunCmd))]
    [XmlInclude(typeof(SendKey))]
    [XmlInclude(typeof(SendText))]
    [XmlInclude(typeof(SleepMilliseconds))]
    [XmlInclude(typeof(BuildinOperation))]
    public abstract class Operation : INotifyPropertyChanged, IComparable
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

        protected abstract bool EmptyParameter();

        protected abstract void _invoke();

        protected virtual void BeforeInvoke()
        {
        }

        public SyncMode Mode { get; set; } = SyncMode.Task;

        public void BeginInvoke()
        {
            BeforeInvoke();
            switch (Mode)
            {
                case SyncMode.Sync:
                    Invoke();
                    return;
                case SyncMode.Task:
                    Task.Run(delegate ()
                    {
                        Invoke();
                    });
                    return;
                case SyncMode.Thread:
                    new Thread(delegate ()
                    {
                        try
                        {
                            Invoke();
                        }
                        catch (Exception e)
                        {
                            Helper.ProcessFatalUnhandledException(e);
                        }
                    }).Start();
                    return;
                case SyncMode.Dispatcher:
                    Application.Current.Dispatcher.Invoke(delegate ()
                    {
                        Invoke();
                    });
                    return;
                default:
                    return;
            }
        }

        private void Invoke()
        {
            if (EmptyParameter())
            {
                Notify.PopNewToast("EmptyOperation", null);
                return;
            }
            G.TriggerCounter();
            _invoke();
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(((Operation)obj).ToString());
        }

        public enum SyncMode
        {
            Sync,
            Task,
            Thread,
            Dispatcher
        }
    }
}
