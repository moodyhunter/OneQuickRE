using System;

namespace OneQuick.Core.Triggers
{
    public class NonMoveTrigger : Trigger
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

        public int Counter { get; private set; }

        public override void ResetState()
        {
            Counter = 0;
            Fired = false;
        }

        public bool TimePassed(int ms)
        {
            if (ms <= 0)
            {
                throw new Exception();
            }
            if (Milliseconds < 0)
            {
                return false;
            }
            Counter += ms;
            if (Fired || Counter < Milliseconds)
            {
                return false;
            }
            if (Condition != null && Condition.Fit())
            {
                TriggerFire();
                Fired = true;
                return true;
            }
            ResetState();
            return false;
        }

        public NonMoveTrigger()
        {
        }

        public NonMoveTrigger(int ms)
        {
            Milliseconds = ms;
        }

        public override string ToString()
        {
            return "!" + (Milliseconds / 1000.0).ToString() + "s";
        }

        private int _milliseconds = -1000;

        private bool Fired;
    }
}
