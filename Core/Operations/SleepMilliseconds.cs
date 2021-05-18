using System.Threading;

namespace OneQuick.Core.Operations
{
    public class SleepMilliseconds : Operation
    {
        public int Milliseconds
        {
            get => _milliseconds;
            set
            {
                _milliseconds = value;
                OnPropertyChanged("Milliseconds");
            }
        }

        public SleepMilliseconds()
        {
        }

        public SleepMilliseconds(int ms)
        {
            Milliseconds = ms;
        }

        protected override bool EmptyParameter()
        {
            return Milliseconds <= 0;
        }

        protected override void _invoke()
        {
            Thread.Sleep(Milliseconds);
        }

        public override string ToString()
        {
            return "Sleep: " + Milliseconds / 1000.0 + "s";
        }

        private int _milliseconds = -1;
    }
}
