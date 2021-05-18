using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System.Collections.Generic;
using System.Linq;

namespace OneQuick.Config
{
    public class QuickSearchGroupCollection : ConfigEntry
    {
        public QuickSearchGroupCollection(IEnumerable<QuickSearchEntry> _quickSearchEntries)
        {
            QuickSearchEntries = _quickSearchEntries;
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            if (QuickSearchEntries == null)
            {
                return null;
            }
            List<List<QuickSearchEntry>> list = (from o in QuickSearchEntries
                                                 where o.Enable && o.GroupKey.KeyValue != 0 && o.LaunchUrl != ""
                                                 group o by o.GroupKey into grp
                                                 select grp.ToList()).ToList();
            List<HotkeyTrigger> list2 = new List<HotkeyTrigger>();
            foreach (List<QuickSearchEntry> list3 in list)
            {
                OperationList operationList = new OperationList();
                K k = K.None;
                foreach (QuickSearchEntry quickSearchEntry in list3)
                {
                    operationList.Add(new RunCmd(quickSearchEntry.LaunchUrl)
                    {
                        ReplaceClipboardTo = "%s",
                        Mode = Operation.SyncMode.Dispatcher
                    });
                    k = quickSearchEntry.GroupKey.KeyData;
                }
                if (k != K.None)
                {
                    list2.Add(new HotkeyTrigger
                    {
                        Sequence = new ObservableCollectionX<HotkeySingle>
                        {
                            new HotkeySingle((K)131139)
                            {
                                Handled = false
                            },
                            new HotkeySingle((K)131139)
                            {
                                Handled = true
                            },
                            new HotkeySingle(k)
                            {
                                Handled = true
                            }
                        },
                        Operation = operationList
                    });
                }
            }
            foreach (HotkeyTrigger hotkeyTrigger in list2)
            {
                hotkeyTrigger.TriggerType = TriggerType.CopySearch;
            }
            return list2;
        }

        private readonly IEnumerable<QuickSearchEntry> QuickSearchEntries;
    }
}
