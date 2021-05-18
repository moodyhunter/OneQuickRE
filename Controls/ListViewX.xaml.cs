using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace OneQuick.Controls
{
    public partial class ListViewX : ListView, INotifyPropertyChanged
    {
        public event Func<object> OnAddingItem;

        public event Action<object> OnRemovingItem;

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

        public bool HasSelected { get; set; }

        public ListViewX()
        {
            InitializeComponent();
            ContextMenu.DataContext = this;
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit.GetType() != typeof(ListBoxItem))
            {
                UnselectAll();
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                HasSelected = SelectedItem != null;
                OnPropertyChanged("HasSelected");
            }
        }

        public void AddItem()
        {
            Func<object> onAddingItem = OnAddingItem;
            object selectedItem = (onAddingItem != null) ? onAddingItem() : null;
            SelectedItem = selectedItem;
        }

        public void DeleteSelected()
        {
            if (OnRemovingItem == null)
            {
                return;
            }
            int selectedIndex = MainListView.SelectedIndex;
            for (int i = MainListView.SelectedItems.Count - 1; i >= 0; i--)
            {
                object obj = MainListView.SelectedItems[i];
                OnRemovingItem?.Invoke(obj);
            }
            MainListView.SelectedIndex = (selectedIndex > MainListView.Items.Count - 1) ? (MainListView.Items.Count - 1) : selectedIndex;
        }

        public void NextItem()
        {
            SelectedIndex++;
            if (SelectedIndex < 1)
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        public void PrevItem()
        {
            SelectedIndex--;
            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }
        }

        private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.Delete)
            {
                DeleteSelected();
                e.Handled = true;
            }
        }

        private void GridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gridViewColumnHeader = e.OriginalSource as GridViewColumnHeader;
            if (gridViewColumnHeader != null && gridViewColumnHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                ListSortDirection direction;
                if (gridViewColumnHeader != lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else if (lastDirection == ListSortDirection.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    direction = ListSortDirection.Ascending;
                }
                string sortBy = gridViewColumnHeader.Column.GetSortBy();
                if (!string.IsNullOrEmpty(sortBy))
                {
                    Sort(sortBy, direction);
                    lastHeaderClicked = gridViewColumnHeader;
                    lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView defaultView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            defaultView.SortDescriptions.Clear();
            SortDescription item = new SortDescription(sortBy, direction);
            defaultView.SortDescriptions.Add(item);
            defaultView.Refresh();
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        private void DeleteSelectedItems(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private GridViewColumnHeader lastHeaderClicked;

        private ListSortDirection lastDirection;
    }
}
