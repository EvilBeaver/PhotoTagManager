using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    abstract class EntityRepository<T> : TableGatewayBase, IDataRepository<T> where T : IPersistable
    {
        private IDatabase _db;
        
        internal EntityRepository(IDatabase db, TableMapping mapping) : base(mapping)
        {
            Mapping.FieldMapping.Insert(0, new FieldMapping()
            {
                DbField = "id",
                ObjectProperty = "Key",
                Type = SimpleFieldType.Integer,
                PropertyFlags = FieldProperties.AutoIncrement | FieldProperties.PrimaryKey
            });

            _db = db;
            
            base.CreateTableIfNeeded(_db);
        }

        #region Entity Hydration methods

        abstract protected T NewInstance();

        private void Hydrate(ref T instance, IQueryReader reader)
        {
            OnHydrate(ref instance, reader);
        }

        protected virtual void OnHydrate(ref T instance, IQueryReader reader)
        {
            var id = new Identifier(reader["id"]);

            instance.Key = id;

            var type = instance.GetType();
            foreach (var field in Mapping.FieldMapping.Where(x=>!x.PropertyFlags.HasFlag(FieldProperties.PrimaryKey)))
            {
                var prop = type.GetProperty(field.ObjectProperty);
                if (prop != null)
                {
                    prop.SetValue(instance, reader[field.DbField], null);
                }
            }
        }

        #endregion

        #region IDataRepository members

        public void Remove(T item)
        {
            if (!item.Key.IsEmpty())
            {
                var delStatement = BuildDeleteStatement("id");
                
                var cmd = new Query();
                cmd.Text = delStatement;
                cmd.Parameters.Add("filterid", item.Key.Value);
                _db.ExecuteCommand(cmd);

            }
        }

        public void Write(T item)
        {
            if (item.Key.IsEmpty())
            {
                var insert = BuildInsertStatement();

                using (var ts = _db.BeginTransaction())
                {
                    var cmd = new Query();
                    cmd.Text = insert;
                    SetQueryParameters(cmd, item);
                    _db.ExecuteCommand(cmd);

                    cmd.Text = String.Format("SELECT last_insert_rowid() FROM [{0}]", Mapping.TableName);
                    cmd.Parameters.Clear();
                    var rowId = _db.ExecuteScalar(cmd);
                    item.Key = new Identifier(rowId);
                    
                    _db.CommitTransaction();
                }

            }
            else
            {
                var update = BuildUpdateStatement("id");
                
                var cmd = new Query();
                cmd.Text = update;
                SetQueryParameters(cmd, item);
                cmd.Parameters.Add("filterid", item.Key.Value);
                _db.ExecuteCommand(cmd);

                
            }
        }

        public T FindByKey(Identifier key)
        {
            if (!key.IsEmpty())
            {
                var select = BuildSelectStatement("id");

                var cmd = new Query();
                cmd.Text = select;
                cmd.Parameters.Add("filterid", key.Value);

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
            else
            {
                return default(T);
            }
        }

        public IEnumerable<T> GetAll()
        {
            var select = BuildSelectStatement();
            var list = new List<T>();

            var cmd = new Query();
            cmd.Text = select;

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

        #endregion
    }
}
