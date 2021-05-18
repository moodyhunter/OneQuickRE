using System.Windows;
using System.Windows.Controls;

namespace OneQuick.Controls
{
    public static class X
    {
        public static void SetSortBy(this GridViewColumn element, string value)
        {
            element.SetValue(SortByProperty, value);
        }

        public static string GetSortBy(this GridViewColumn element)
        {
            return (string)element.GetValue(SortByProperty);
        }

        public static readonly DependencyProperty SortByProperty = DependencyProperty.Register("SortBy", typeof(string), typeof(GridViewColumn), new PropertyMetadata(null));
    }
}
