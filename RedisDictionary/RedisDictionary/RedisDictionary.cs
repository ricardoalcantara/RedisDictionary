using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDictionary
{
    public class RedisDictionary : IDictionary<string, string>
    {
        ConnectionMultiplexer redis;
        IDatabase db;

        string server = string.Empty;

        string dictionaryId = string.Empty;

        public RedisDictionary()
            : this("localhost", Guid.NewGuid().ToString())
        {

        }

        public RedisDictionary(string server, string dictionaryId)
        {
            this.server = server;
            this.redis = ConnectionMultiplexer.Connect(this.server);

            this.db = redis.GetDatabase();

            this.dictionaryId = dictionaryId;
        }

        public void Add(string key, string value)
        {
            this.db.HashSet(this.dictionaryId, key, value);
        }

        public bool ContainsKey(string key)
        {
            return this.db.HashExists(this.dictionaryId, key);
        }

        ICollection<string> IDictionary<string, string>.Keys
        {
            get { return this.db.HashKeys(this.dictionaryId).Select(x => x.ToString()).ToList(); }
        }

        public IEnumerable<string> Keys
        {
            get { return this.db.HashKeys(this.dictionaryId).Select(x => x.ToString()); }
        }

        public bool Remove(string key)
        {
            return this.db.HashDelete(this.dictionaryId, key);
        }

        public bool TryGetValue(string key, out string value)
        {
            RedisValue val = this.db.HashGet(this.dictionaryId, key);

            value = null;
            if (val.HasValue)
            {
                value = val;
            }

            return val.HasValue;
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string key]
        {
            get
            {
                return this.db.HashGet(this.dictionaryId, key);
            }
            set
            {
                this.db.HashSet(this.dictionaryId, key, value);
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            this.db.HashSet(this.dictionaryId, item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            RedisValue val = this.db.HashGet(this.dictionaryId, item.Key);

            return val.HasValue && val == item.Value;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            var keys = this.db.HashKeys(this.dictionaryId).Select(x => x.ToString());

            foreach (string key in keys)
            {
                yield return new KeyValuePair<string, string>(key, this.db.HashGet(this.dictionaryId, key));
            }
        }

        public void Clear()
        {
            this.db.KeyDelete(this.dictionaryId);
        }

        public int Count
        {
            get { return this.db.HashKeys(this.dictionaryId).Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
