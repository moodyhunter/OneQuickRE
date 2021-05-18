using System.Windows.Controls;
using System.Windows.Markup;

namespace OneQuick.Controls
{
    public partial class PositionWrapperControl : UserControl, IComponentConnector
    {
        public bool ShowCenter { get; set; }

        public PositionWrapperControl()
        {
            InitializeComponent();
        }
    }
}
