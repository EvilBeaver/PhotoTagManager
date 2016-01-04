using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    abstract class RegistryRepository<T>
    {
        private IDatabase _db;
        private TableMapping _mapping;
        private FieldMapping[] _keyFields;
        private TableManager _table;

        internal RegistryRepository(IDatabase db, TableMapping mapping)
        {
            _db = db;
            _mapping = mapping;

            var keys = _mapping.FieldMapping.Where(x=>x.PropertyFlags.HasFlag(FieldProperties.PrimaryKey)).ToArray();
            if (keys.Length == 0)
            {
                throw new ArgumentException("No primary key defined");
            }

            _keyFields = keys;
            _table = new TableManager(_mapping);
            _table.CreateTableIfNeeded(_db);

        }

        public RegistryKey CreateKey()
        {
            var dict = new Dictionary<string, object>();
            foreach (var keyField in _keyFields)
            {
                dict.Add(keyField.ObjectProperty, null);
            }

            return new RegistryKey(dict);
        }

        private string BuildDeleteStatement()
        {
            return _table.BuildDeleteStatement(CreateFilterByKeys());
        }

        private string BuildSelectStatement(bool filtered)
        {
            if (filtered)
            {
                return _table.BuildSelectStatement(CreateFilterByKeys());
            }
            else
            {
                return _table.BuildSelectStatement();
            }
        }

        private string BuildInsertStatement()
        {
            return _table.BuildInsertStatement();
        }

        private string[] CreateFilterByKeys()
        {
            return _keyFields.Select(x => x.DbField).ToArray();
        }

        public void Write(T item)
        {
            using (var transaction = _db.BeginTransaction())
            {
                var cmd = new Query();
                cmd.Text = BuildDeleteStatement();
                _table.SetQueryParameters(cmd, item);
                _db.ExecuteCommand(cmd);

                cmd.Text = BuildInsertStatement();
                _db.ExecuteCommand(cmd);
                _db.CommitTransaction();
            }
            
        }

        public void Remove(RegistryKey key)
        {
            var cmd = new Query();
            cmd.Text = BuildDeleteStatement();
            SetParametersByRecordKey(key, cmd);
            _db.ExecuteCommand(cmd);

        }

        public T FindByKey(RegistryKey key)
        {
            var cmd = new Query();
            cmd.Text = BuildSelectStatement(true);
            SetParametersByRecordKey(key, cmd);

            using (var reader = _db.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    reader.ReadNext();
                    var item = NewInstance();
                    Hydrate(ref item, reader);
                    return item;
                }
                else
                {
                    return default(T);
                }
            }
            
        }

        private void SetParametersByRecordKey(RegistryKey key, Query cmd)
        {
            foreach (var keyItem in _keyFields)
            {
                object keyVal = key[keyItem.ObjectProperty];
                if (keyVal.GetType() == typeof(Identifier))
                {
                    keyVal = ((Identifier)keyVal).Value;
                }
                cmd.Parameters.Add(keyItem.DbField, keyVal);
            }
        }

        abstract protected T NewInstance();

        private void Hydrate(ref T instance, IQueryReader reader)
        {
            OnHydrate(ref instance, reader);
        }

        protected virtual void OnHydrate(ref T instance, IQueryReader reader)
        {
            var type = instance.GetType();
            foreach (var field in _mapping.FieldMapping)
            {
                var prop = type.GetProperty(field.ObjectProperty);
                prop.SetValue(instance, reader[field.DbField], null);
            }
        }


        public IEnumerable<T> GetAll()
        {
            var select = BuildSelectStatement(false);
            var list = new List<T>();

            var cmd = new Query();
            cmd.Text = select.ToString();
            using (var reader = _db.ExecuteReader(cmd))
            {
                while (reader.ReadNext())
                {
                    var item = NewInstance();
                    Hydrate(ref item, reader);
                    list.Add(item);
                }
            }
            
            return list;
        }

    }

    class RegistryKey : IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> _keys;

        public RegistryKey(Dictionary<string, object> keys)
        {
            _keys = new Dictionary<string, object>();
            foreach (var item in keys)
            {
                _keys.Add(item.Key, item.Value);
            }
        }

        public object this[string key] 
        {
            get
            {
                return _keys[key];
            }
            set
            {
                if (!_keys.ContainsKey(key))
                {
                    throw new ArgumentException(string.Format("Key is not found {0}", key));
                }

                _keys[key] = value;
            }
        }

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in _keys)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}
