using OneQuick.Notification;
using OneQuick.SysX;
using System;

namespace OneQuick.Core.Operations
{
    public class BuildinOperation : Operation
    {
        public BuildinOperationEnum OperationEnum
        {
            get => _winOp;
            set
            {
                _winOp = value;
                OnPropertyChanged("OperationEnum");
            }
        }

        public BuildinOperation()
        {
            OperationEnum = BuildinOperationEnum.Empty;
        }

        public BuildinOperation(BuildinOperationEnum op)
        {
            OperationEnum = op;
        }

        protected override bool EmptyParameter()
        {
            return OperationEnum == BuildinOperationEnum.Empty;
        }

        protected override void BeforeInvoke()
        {
            base.BeforeInvoke();
            if (OperationEnum == BuildinOperationEnum.ShowInfoWindow)
            {
                Mode = SyncMode.Dispatcher;
            }
        }

        protected override void _invoke()
        {
            switch (OperationEnum)
            {
                case BuildinOperationEnum.Empty:
                case BuildinOperationEnum.Block:
                    return;
                case BuildinOperationEnum.ToggleTopmost:
                    Win.ToggleTopmost(null, null);
                    return;
                case BuildinOperationEnum.OpacityDown:
                    Win.SetOpacity(Math.Max(0.25, Win.GetOpacity(null) - 0.05), null);
                    return;
                case BuildinOperationEnum.OpacityUp:
                    Win.SetOpacity(Math.Min(1.0, Win.GetOpacity(null) + 0.05), null);
                    return;
                case BuildinOperationEnum.ShowInfoWindow:
                    WindowInfo.ShowInfo(null);
                    return;
                case BuildinOperationEnum.ExplorerAppPath:
                    Cmd.Explorer(Win.GetWindowProcFileName(null));
                    return;
                case BuildinOperationEnum.MonitorOff:
                    Win.MonitorOff();
                    return;
                case BuildinOperationEnum.Suspend:
                    NativeMethods.SetSuspendState(false, false, false);
                    return;
                case BuildinOperationEnum.Hibernate:
                    NativeMethods.SetSuspendState(true, false, false);
                    return;
                case BuildinOperationEnum.ShowQuickSearchWindow:
                    Notify.ShowMsg("//TODO ShowQuickSearchWindow", "");
                    return;
                default:
                    throw new Exception();
            }
        }

        public override string ToString()
        {
            return OperationEnum.ToString().ToLower();
        }

        private BuildinOperationEnum _winOp;
    }
}
