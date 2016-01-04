using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    abstract class EntityRepository<T> : IDataRepository<T> where T : IPersistable
    {
        private TableMapping _mapping;
        private IDatabase _db;
        private TableManager _table;
        
        internal EntityRepository(IDatabase db, TableMapping mapping)
        {
            _mapping = new TableMapping();
            _mapping.TableName = mapping.TableName;
            _mapping.FieldMapping.Add(new FieldMapping()
            {
                DbField = "id",
                ObjectProperty = "Key",
                Type = SimpleFieldType.Integer,
                PropertyFlags = FieldProperties.AutoIncrement | FieldProperties.PrimaryKey
            });

            foreach (var item in mapping.FieldMapping)
            {
                _mapping.FieldMapping.Add(item);
            }

            _db = db;
            _table = new TableManager(_mapping);
            _table.CreateTableIfNeeded(_db);
        }

        private string BuildSelectStatement(params string[] filter)
        {
            return _table.BuildSelectStatement(filter);
        }

        private string BuildUpdateStatement(params string[] filter)
        {
            return _table.BuildUpdateStatement(filter);
        }

        private string BuildInsertStatement()
        {

            return _table.BuildInsertStatement();

        }

        private string BuildDeleteStatement(params string[] filter)
        {
            return _table.BuildDeleteStatement();
        }

        private void AppendFilter(StringBuilder sb, string[] filterColumns)
        {
            if(filterColumns.Length == 0)
            {
                return;
            }

            sb.AppendLine("\nWHERE");

            for (int i = 0; i < filterColumns.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append("\nAND ");
                }

                sb.AppendFormat("{0} = @filter{0}", filterColumns[i]);
            }
        }

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
            foreach (var field in _mapping.FieldMapping)
	        {
                var prop = type.GetProperty(field.ObjectProperty);
                prop.SetValue(instance, reader[field.DbField], null);
	        }
        }

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
                    _table.SetQueryParameters(cmd, item);
                    _db.ExecuteCommand(cmd);

                    cmd.Text = String.Format("SELECT last_insert_rowid() FROM [{0}]", _mapping.TableName);
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
                _table.SetQueryParameters(cmd, item);
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
