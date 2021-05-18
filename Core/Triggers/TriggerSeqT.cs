namespace OneQuick.Core.Triggers
{
    public abstract class TriggerSeqT<TSeq, TItem> : Trigger
    {
        public event SequenceSetDelegate OnSequenceSetted;

        public TSeq Sequence
        {
            get => _sequnce;
            set
            {
                OnSequenceSetted?.Invoke(_sequnce, value);
                _sequnce = value;
                OnPropertyChanged("Sequence");
            }
        }

        public override void ResetState()
        {
            Ptr = 0;
        }

        public abstract int GetLength(TSeq seq);

        public abstract bool ItemFitSlot(TItem slot, TItem input);

        public abstract bool SeqFitSlot(TSeq slot, TSeq inputChain);

        public abstract TItem GetSeqItem(TSeq seq, int index);

        public abstract TItem GetSeqLastItem(TSeq seq);

        public abstract TSeq GetSubSeqFromHead(TSeq seq, int n);

        public abstract TSeq GetSubSeqFromTail(TSeq seq, int n);

        public int TestInput(TSeq inputSeq, int idx)
        {
            if (idx < 0)
            {
                return 0;
            }
            if (idx > GetLength(Sequence))
            {
                return GetLength(Sequence);
            }
            if (GetLength(inputSeq) == 0)
            {
                return idx;
            }
            TItem seqItem = GetSeqItem(Sequence, idx);
            TItem seqLastItem = GetSeqLastItem(inputSeq);
            if (ItemFitSlot(seqItem, seqLastItem))
            {
                return idx + 1;
            }
            while (idx > 0)
            {
                TSeq subSeqFromHead = GetSubSeqFromHead(Sequence, idx);
                TSeq subSeqFromTail = GetSubSeqFromTail(inputSeq, idx);
                if (SeqFitSlot(subSeqFromHead, subSeqFromTail))
                {
                    return idx;
                }
                idx--;
            }
            return idx;
        }

        private TSeq _sequnce;

        protected int Ptr;

        // (Invoke) Token: 0x06000AC6 RID: 2758
        public delegate void SequenceSetDelegate(TSeq oldSeq, TSeq newSeq);
    }
}
