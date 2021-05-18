using System.Collections;
using System.Collections.Generic;

namespace OneQuick.Core.Operations
{
    public class OperationList : Operation, IEnumerable<Operation>, IEnumerable
    {
        public ObservableCollectionX<Operation> Operations { get; set; }

        public void Add(Operation operation)
        {
            Operations.Add(operation);
        }

        public OperationList()
        {
            Mode = SyncMode.Sync;
            Operations = new ObservableCollectionX<Operation>();
        }

        public OperationList(params Operation[] operation_array) : this()
        {
            Operations = new ObservableCollectionX<Operation>();
            foreach (Operation item in operation_array)
            {
                Operations.Add(item);
            }
        }

        protected override bool EmptyParameter()
        {
            return Operations.Count == 0;
        }

        protected override void _invoke()
        {
            foreach (Operation operation in Operations)
            {
                operation.BeginInvoke();
            }
        }

        public override string ToString()
        {
            if (Operations.Count == 0)
            {
                return "<Empty>";
            }
            return string.Join(", ", Operations);
        }

        public IEnumerator<Operation> GetEnumerator()
        {
            return Operations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Operations.GetEnumerator();
        }
    }
}
