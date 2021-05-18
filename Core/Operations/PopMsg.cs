using OneQuick.Notification;

namespace OneQuick.Core.Operations
{
    public class PopMsg : Operation
    {
        public string Msg
        {
            get => _msg;
            set
            {
                _msg = value;
                OnPropertyChanged("Msg");
            }
        }

        public PopMsg()
        {
        }

        public PopMsg(string s)
        {
            Msg = s;
        }

        protected override bool EmptyParameter()
        {
            return Msg == null || Msg == "";
        }

        protected override void _invoke()
        {
            Notify.ShowMsg(Msg, "");
        }

        public override string ToString()
        {
            return "Msg: " + Msg;
        }

        private string _msg;
    }
}
