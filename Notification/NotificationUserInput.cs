using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OneQuick.Notification
{
    public class NotificationUserInput : IReadOnlyDictionary<string, string>, IReadOnlyCollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        internal NotificationUserInput(NotificationActivator.NOTIFICATION_USER_INPUT_DATA[] data)
        {
            _data = data;
        }

        public string this[string key]
        {
            get
            {
                return _data.First((NotificationActivator.NOTIFICATION_USER_INPUT_DATA i) => i.Key == key).Value;
            }
        }

        public IEnumerable<string> Keys => from i in _data
                                           select i.Key;

        public IEnumerable<string> Values => from i in _data
                                             select i.Value;

        public int Count => _data.Length;

        public bool ContainsKey(string key)
        {
            return _data.Any((NotificationActivator.NOTIFICATION_USER_INPUT_DATA i) => i.Key == key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return (from i in _data
                    select new KeyValuePair<string, string>(i.Key, i.Value)).GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            foreach (NotificationActivator.NOTIFICATION_USER_INPUT_DATA notification_USER_INPUT_DATA in _data)
            {
                if (notification_USER_INPUT_DATA.Key == key)
                {
                    value = notification_USER_INPUT_DATA.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly NotificationActivator.NOTIFICATION_USER_INPUT_DATA[] _data;
    }
}
