using OneQuick.Core.Operations;
using System.Windows;
using System.Windows.Controls;

namespace OneQuick.Controls
{
    public partial class OperationsControl : UserControl
    {
        public Operation Operations
        {
            get => (Operation)GetValue(OperationsProperty);
            set => SetValue(OperationsProperty, value);
        }

        public OperationList OperationList
        {
            get
            {
                if (Operations == null)
                {
                    return null;
                }
                if (Operations is OperationList)
                {
                    return (OperationList)Operations;
                }
                return new OperationList(new Operation[]
                {
                    Operations
                });
            }
            set => Operations = value;
        }

        public OperationsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty OperationsProperty = DependencyProperty.Register("Operations", typeof(Operation), typeof(OperationsControl), new PropertyMetadata(null));
    }
}
