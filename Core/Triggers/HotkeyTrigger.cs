using OneQuick.WindowsSimulator;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace OneQuick.Core.Triggers
{
    public class HotkeyTrigger : TriggerSeqT<ObservableCollectionX<HotkeySingle>, HotkeySingle>, IComparable
    {
        public bool KeydownTriggered { get; set; } = true;

        [XmlIgnore]
        public K TheK
        {
            get
            {
                if (Sequence != null && Sequence.Count > 0)
                {
                    return Sequence[0].KeyData;
                }
                return K.None;
            }
            set
            {
                if (Sequence == null)
                {
                    Sequence = new ObservableCollectionX<HotkeySingle>();
                }
                Sequence.Clear();
                Sequence.Add(new HotkeySingle(value));
            }
        }

        public HotkeyTrigger()
        {
            OnSequenceSetted += HotkeyTrigger_OnSequenceSetted;
            Sequence = new ObservableCollectionX<HotkeySingle>();
        }

        private void HotkeyTrigger_OnSequenceSetted(ObservableCollectionX<HotkeySingle> oldSeq, ObservableCollectionX<HotkeySingle> newSeq)
        {
            if (oldSeq != null)
            {
                newSeq.ItemsPropertyChanged -= NewSeq_ItemsPropertyChanged;
                newSeq.CollectionChanged -= NewSeq_CollectionChanged;
            }
            if (newSeq != null)
            {
                newSeq.ItemsPropertyChanged += NewSeq_ItemsPropertyChanged;
                newSeq.CollectionChanged += NewSeq_CollectionChanged;
            }
        }

        private void NewSeq_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Sequence");
        }

        private void NewSeq_ItemsPropertyChanged(object sender, object item, string PropertyName)
        {
            OnPropertyChanged("Sequence");
        }

        public HotkeyTrigger(K k) : this()
        {
            Sequence.Clear();
            Sequence.Add(new HotkeySingle(k));
        }

        public HotkeyTrigger(Kwrapper kw) : this(kw.KeyData)
        {
        }

        public bool EventForward(ObservableCollectionX<HotkeySingle> inputList, bool isKeyDown, out bool handle)
        {
            handle = false;
            if (Sequence.Count == 0)
            {
                return false;
            }
            if (Ptr == Sequence.Count - 1 && KeydownTriggered != isKeyDown)
            {
                return false;
            }
            if (Ptr < Sequence.Count - 1 && !isKeyDown)
            {
                return false;
            }
            int num = TestInput(inputList, Ptr);
            int num2 = num - 1;
            if (num2 >= 0 && num2 < Sequence.Count)
            {
                if (Sequence.Count > 1)
                {
                    handle = Sequence[num2].Handled;
                }
                if ((isKeyDown & handle) && Sequence[num2].KeyData.Win())
                {
                    SimulatorServer.KeyDown(K.LControlKey);
                    SimulatorServer.KeyUp(K.LControlKey);
                    EntryServer.HandleNextWinUp();
                }
            }
            if (num < Sequence.Count)
            {
                Ptr = num;
                return false;
            }
            if (Condition == null || Condition.Fit())
            {
                handle = Sequence.Last().Handled;
                if ((isKeyDown & handle) && Sequence.Last().KeyData.Win())
                {
                    SimulatorServer.KeyDown(K.LControlKey);
                    SimulatorServer.KeyUp(K.LControlKey);
                    EntryServer.HandleNextWinUp();
                }
                TriggerFire();
                ResetState();
                return true;
            }
            Ptr = TestInput(inputList, Ptr - 1);
            return false;
        }

        public override int GetLength(ObservableCollectionX<HotkeySingle> seq)
        {
            return Sequence.Count;
        }

        public override bool ItemFitSlot(HotkeySingle slot, HotkeySingle input)
        {
            return slot.KeyData == input.KeyData && (slot.ModSide == ModSide.Both || slot.ModSide == input.ModSide || !slot.KeyData.ModifiersKeyPressed());
        }

        public override bool SeqFitSlot(ObservableCollectionX<HotkeySingle> slot, ObservableCollectionX<HotkeySingle> inputSeq)
        {
            if (slot.Count != inputSeq.Count)
            {
                return false;
            }
            for (int i = 0; i < slot.Count; i++)
            {
                if (!ItemFitSlot(slot[i], inputSeq[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override HotkeySingle GetSeqItem(ObservableCollectionX<HotkeySingle> seq, int index)
        {
            return seq[index];
        }

        public override HotkeySingle GetSeqLastItem(ObservableCollectionX<HotkeySingle> seq)
        {
            return seq.Last();
        }

        public override ObservableCollectionX<HotkeySingle> GetSubSeqFromHead(ObservableCollectionX<HotkeySingle> seq, int n)
        {
            if (n > seq.Count)
            {
                throw new IndexOutOfRangeException();
            }
            ObservableCollectionX<HotkeySingle> observableCollectionX = new ObservableCollectionX<HotkeySingle>();
            for (int i = 0; i < n; i++)
            {
                observableCollectionX.Add(seq[i]);
            }
            return observableCollectionX;
        }

        public override ObservableCollectionX<HotkeySingle> GetSubSeqFromTail(ObservableCollectionX<HotkeySingle> seq, int n)
        {
            if (n > seq.Count)
            {
                throw new IndexOutOfRangeException();
            }
            ObservableCollectionX<HotkeySingle> observableCollectionX = new ObservableCollectionX<HotkeySingle>();
            for (int i = seq.Count - n; i < seq.Count; i++)
            {
                observableCollectionX.Add(seq[i]);
            }
            return observableCollectionX;
        }

        public override string ToString()
        {
            return ((Sequence == null) ? "" : string.Join(" ", Sequence)) ?? "";
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(((HotkeyTrigger)obj).ToString());
        }
    }
}
