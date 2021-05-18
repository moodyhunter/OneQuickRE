using System.Collections.Generic;
using System.Linq;

namespace OneQuick.Config
{
    public static class KeyMappingX
    {
        public static Dictionary<int, int> ToDictionaryIntInt(this ObservableCollectionX<KeyMappingItem> list)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            if (list == null)
            {
                return dictionary;
            }
            if (G.MainWindow.LitePrivilege)
            {
                if ((from o in list
                     where o.Enable
                     select o).Count() > 1)
                {
                    return dictionary;
                }
            }
            foreach (KeyMappingItem keyMappingItem in list)
            {
                if (keyMappingItem.Enable && keyMappingItem.Key.KeyValue != 0)
                {
                    dictionary[keyMappingItem.Key.KeyValue] = keyMappingItem.Value.KeyValue;
                }
            }
            return dictionary;
        }

        public static ObservableCollectionX<KeyMappingItem> ToKeyMappingCollection(this Dictionary<int, int> dict)
        {
            ObservableCollectionX<KeyMappingItem> observableCollectionX = new ObservableCollectionX<KeyMappingItem>();
            if (dict == null)
            {
                return observableCollectionX;
            }
            foreach (KeyValuePair<int, int> keyValuePair in dict)
            {
                if (keyValuePair.Key != 0)
                {
                    observableCollectionX.Add(new KeyMappingItem
                    {
                        Key = new Kwrapper(keyValuePair.Key),
                        Value = new Kwrapper(keyValuePair.Value)
                    });
                }
            }
            return observableCollectionX;
        }
    }
}
