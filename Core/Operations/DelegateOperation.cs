using System;

namespace OneQuick.Core.Operations
{
    public class DelegateOperation : Operation
    {
        public Action Action
        {
            get => _action;
            private set
            {
                _action = value;
                OnPropertyChanged("Action");
            }
        }

        public DelegateOperation(Action act)
        {
            Action = act;
        }

        protected override bool EmptyParameter()
        {
            return Action == null;
        }

        protected override void _invoke()
        {
            Action action = Action;
            if (action == null)
            {
                return;
            }
            action();
        }

        public override string ToString()
        {
            return "Delegate: " + Action.ToString();
        }

        private Action _action;
    }
}
