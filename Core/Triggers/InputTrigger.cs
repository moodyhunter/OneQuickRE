using System.Linq;

namespace OneQuick.Core.Triggers
{
    public class InputTrigger : TriggerSeqT<string, char>
    {
        public InputTrigger()
        {
            Sequence = "";
        }

        public InputTrigger(string input)
        {
            Sequence = input;
        }

        public override string ToString()
        {
            return "\"" + Sequence + "\"";
        }

        public void Backspace()
        {
            Ptr--;
            if (Ptr < 0)
            {
                Ptr = 0;
            }
        }

        public bool EventForward(string inputSequence)
        {
            if (Sequence.Length == 0)
            {
                return false;
            }
            int num = TestInput(inputSequence, Ptr);
            if (num < Sequence.Length)
            {
                Ptr = num;
                return false;
            }
            if (Condition == null || Condition.Fit())
            {
                TriggerFire();
                ResetState();
                return true;
            }
            Ptr = TestInput(inputSequence, Ptr - 1);
            return false;
        }

        public override bool ItemFitSlot(char item1, char item2)
        {
            return item1 == item2;
        }

        public override bool SeqFitSlot(string seq1, string seq2)
        {
            return seq1 == seq2;
        }

        public override char GetSeqItem(string seq, int index)
        {
            return seq[index];
        }

        public override char GetSeqLastItem(string seq)
        {
            return seq.Last();
        }

        public override string GetSubSeqFromHead(string seq, int n)
        {
            return seq.Substring(0, n);
        }

        public override string GetSubSeqFromTail(string seq, int n)
        {
            return seq.Substring(seq.Length - n);
        }

        public override int GetLength(string seq)
        {
            return seq.Length;
        }
    }
}
