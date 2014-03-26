using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    public abstract class RegistryRepository<T>
    {
        private IDatabase _db;
        private TableMapping _mapping;
        private FieldMapping[] _keyFields;

        internal RegistryRepository(IDatabase db, TableMapping mapping)
        {
            _db = db;
            _mapping = mapping;

            var keys = (from fields in _mapping.FieldMapping where fields.Indexed == FieldIndex.PrimaryKey select fields).ToArray();
            if (keys.Length == 0)
            {
                throw new ArgumentException("No primary key defined");
            }

            _keyFields = keys;
            
            CreateTableIfNeeded();

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

        private void CreateTableIfNeeded()
        {
            using (var con = _db.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'",
                        _mapping.TableName);
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (!reader.HasRows)
                        {
                            CreateTable(con);
                        }
                    }
                }
            }

        }

        private void CreateTable(SQLiteConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                var sb = new StringBuilder();
                var indexSb = new StringBuilder();

                sb.AppendFormat("CREATE TABLE {0} (", _mapping.TableName);
                bool isFirst = true;
                foreach (var field in _mapping.FieldMapping)
                {
                    if (!isFirst)
                    {
                        sb.Append(',');
                    }

                    sb.AppendFormat("\n [{0}] {1} NOT NULL", field.DbField, TypeExpression(field));
                    
                                    
                    isFirst = false;

                    if (field.Indexed == FieldIndex.Unique || field.Indexed == FieldIndex.NotUnique)
                    {
                        indexSb.AppendFormat("CREATE {0} INDEX {1} ON {2} ({3});\n",
                            field.Indexed == FieldIndex.Unique ? "UNIQUE" : "",
                            "idx_" + field.DbField,
                            _mapping.TableName,
                            field.DbField);
                    }
                }

                sb.Append(",\nPRIMARY KEY (");
                for (int i = 0; i < _keyFields.Length; i++)
                {
                    
                    if (i > 0)
                    {
                        sb.Append(',');
                    }

                    sb.AppendFormat("[{0}]",_keyFields[i].DbField);

                }

                sb.Append(")\n);");
                sb.AppendLine(indexSb.ToString());
                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();
            }
        }

        private string TypeExpression(FieldMapping field)
        {
            switch (field.Type)
            {
                case SimpleFieldType.Integer:
                    return "integer";
                case SimpleFieldType.Double:
                    return "real";
                case SimpleFieldType.Date:
                    return "integer";
                case SimpleFieldType.String:
                    return string.Format("varchar({0})", field.Length);
                default:
                    return "";
            }
        }


        private string BuildDeleteStatement()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM [{0}]\n", _mapping.TableName);
            AppendFilterByKeys(sb);
            sb.Append(';');

            return sb.ToString();
        }

        private StringBuilder BuildSelectStatement()
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            bool isFirst = true;
            foreach (var field in _mapping.FieldMapping)
            {
                if (!isFirst)
                {
                    sb.Append(',');
                }

                sb.AppendFormat("[{0}]", field.DbField);
                isFirst = false;
            }

            sb.AppendFormat("\nFROM [{0}]", _mapping.TableName);

            return sb;
        }

        private string BuildInsertStatement()
        {
            var sb = new StringBuilder();
            var valSb = new StringBuilder();
            sb.AppendFormat("INSERT INTO [{0}]", _mapping.TableName);
            valSb.Append("VALUES ");
            bool isFirst = true;
            sb.Append(" (\n");
            valSb.Append(" (\n");
            foreach (var field in _mapping.FieldMapping)
            {
                if (!isFirst)
                {
                    sb.Append(",\n");
                    valSb.Append(",\n");
                }
                sb.AppendFormat("[{0}]", field.DbField);
                valSb.AppendFormat("@{0}", field.DbField);
                isFirst = false;
            }

            sb.Append(")\n");
            valSb.Append(")");
            sb.Append(valSb.ToString());
            sb.Append(';');

            return sb.ToString();
        }

        private void AppendFilterByKeys(StringBuilder sb)
        {
            sb.AppendLine("\nWHERE");
            for (int i = 0; i < _keyFields.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append("AND ");
                }

                sb.AppendFormat("[{0}] = @{0}\n", _keyFields[i].DbField);
            }
        }

        private void SetNamedParameters(SQLiteCommand cmd, object data)
        {
            var type = data.GetType();
            foreach (var field in _mapping.FieldMapping)
            {
                var prop = type.GetProperty(field.ObjectProperty);
                object paramValue;
                if(prop.PropertyType == typeof(Identifier))
                {
                    var id = (Identifier)prop.GetValue(data, null);
                    paramValue = id.Value;
                }
                else
                {
                    paramValue = prop.GetValue(data, null);
                }

                cmd.Parameters.AddWithValue("@" + field.DbField, paramValue);

            }
        }

        public void Write(T item)
        {
            using (var con = _db.OpenConnection())
            {
                using(var transaction = con.BeginTransaction())
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = BuildDeleteStatement();
                    SetNamedParameters(cmd, item);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = BuildInsertStatement();
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                }
            }
        }

        public void Remove(RegistryKey key)
        {
            using (var con = _db.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = BuildDeleteStatement();

                    SetParametersByRecordKey(key, cmd);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public T FindByKey(RegistryKey key)
        {
            using (var con = _db.OpenConnection())
            using (var cmd = con.CreateCommand())
            {
                var sb = BuildSelectStatement();
                AppendFilterByKeys(sb);

                SetParametersByRecordKey(key, cmd);

                cmd.CommandText = sb.ToString();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
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
            

        }

        private void SetParametersByRecordKey(RegistryKey key, SQLiteCommand cmd)
        {
            foreach (var keyItem in _keyFields)
            {
                object keyVal = key[keyItem.ObjectProperty];
                if (keyVal.GetType() == typeof(Identifier))
                {
                    keyVal = ((Identifier)keyVal).Value;
                }
                cmd.Parameters.AddWithValue("@" + keyItem.DbField, keyVal);
            }
        }

        abstract protected T NewInstance();

        private void Hydrate(ref T instance, SQLiteDataReader reader)
        {
            OnHydrate(ref instance, reader);
        }

        protected virtual void OnHydrate(ref T instance, SQLiteDataReader reader)
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
            var select = BuildSelectStatement();
            var list = new List<T>();
            using (var con = _db.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = NewInstance();
                            Hydrate(ref item, reader);
                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

    }

    public class RegistryKey : IEnumerable<KeyValuePair<string, object>>
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
