using OneQuick.WindowsSimulator;

namespace OneQuick.Core.Operations
{
    public class SendText : Operation
    {
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public int BackspaceCount
        {
            get => _backspaceCount;
            set
            {
                _backspaceCount = value;
                OnPropertyChanged("BackspaceCount");
            }
        }

        public SendText()
        {
            Mode = SyncMode.Task;
        }

        public SendText(string s, int back = 0) : this()
        {
            Text = s;
            BackspaceCount = back;
        }

        protected override bool EmptyParameter()
        {
            return Text == null || Text == "";
        }

        protected override void _invoke()
        {
            SimulatorServer.SendText(Text, BackspaceCount);
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"", Text);
        }

        private string _text;

        private int _backspaceCount;
    }
}
