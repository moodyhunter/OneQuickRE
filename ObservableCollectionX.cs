using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OneQuick
{
    [Serializable]
    public class ObservableCollectionX<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event ItemsPropertyChangedEventHandler ItemsPropertyChanged;

        public ObservableCollectionX()
        {
            CollectionChanged += ObservableCollectionX_CollectionChanged;
        }

        private void ObservableCollectionX_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object obj in e.NewItems)
                {
                    ((INotifyPropertyChanged)obj).PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (object obj2 in e.OldItems)
                {
                    ((INotifyPropertyChanged)obj2).PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemsPropertyChangedEventHandler itemsPropertyChanged = ItemsPropertyChanged;
            if (itemsPropertyChanged == null)
            {
                return;
            }
            itemsPropertyChanged(this, sender, e.PropertyName);
        }

        // (Invoke) Token: 0x06000A1F RID: 2591
        public delegate void ItemsPropertyChangedEventHandler(object sender, object item, string PropertyName);
    }
}
