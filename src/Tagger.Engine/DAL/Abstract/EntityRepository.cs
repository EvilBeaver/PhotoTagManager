using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    public abstract class EntityRepository<T> : IDataRepository<T> where T : IPersistable
    {
        private TableMapping _mapping;
        private IDatabase _db;
        
        internal EntityRepository(IDatabase db, TableMapping mapping)
        {
            _mapping = mapping;
            _db = db;
            CreateTableIfNeeded();
        }

        private void CreateTableIfNeeded()
        {
            var query = new Query();
            query.Text = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'",
                        _mapping.TableName);
            using (var reader = _db.ExecuteReader(query))
            {
                if (!reader.HasRows)
                {
                    CreateTable();
                }
            }
            
        }

        private void CreateTable()
        {
            var sb = new StringBuilder();
            var indexSb = new StringBuilder();

            sb.AppendFormat("CREATE TABLE {0} (\n", _mapping.TableName);
            sb.Append("id integer PRIMARY KEY AUTOINCREMENT NOT NULL");
            foreach (var field in _mapping.FieldMapping)
            {
                sb.AppendFormat(",\n {0} {1} NOT NULL", field.DbField, TypeExpression(field));
                if (field.Indexed != FieldIndex.None)
                {
                    indexSb.AppendFormat("CREATE {0} INDEX {1} ON {2} ({3});\n",
                        field.Indexed == FieldIndex.Unique? "UNIQUE" : "",
                        "idx_"+field.DbField,
                        _mapping.TableName,
                        field.DbField);
                }
            }
            sb.Append(");\n");
            sb.AppendLine(indexSb.ToString());

            var query = new Query();
            query.Text = sb.ToString();
            
            _db.ExecuteCommand(query);

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

        private StringBuilder BuildSelectStatement()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT\n");
            sb.AppendFormat("[{0}]", "id");
            foreach (var field in _mapping.FieldMapping)
            {
                sb.AppendFormat(",\n[{0}]", field.DbField);
            }

            sb.AppendFormat("\nFROM [{0}]", _mapping.TableName);

            return sb;
        }

        private StringBuilder BuildUpdateStatement()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0}\n", _mapping.TableName);
            bool isFirst = true;
            foreach (var field in _mapping.FieldMapping)
            {
                if (!isFirst)
                {
                    sb.Append(",\n");
                }
                sb.AppendFormat("SET {0} = @{0}", field.DbField);
                isFirst = false;
            }

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
                sb.AppendFormat("[{0}]",  field.DbField);
                valSb.AppendFormat("@{0}", field.DbField);
                isFirst = false;
            }

            sb.Append(")\n");
            valSb.Append(")");
            sb.Append(valSb.ToString());
            sb.Append(';');

            return sb.ToString();
        }

        private StringBuilder BuildDeleteStatement()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0}", _mapping.TableName);
            return sb;
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

        private void SetNamedParameters(Query query, object data, string paramPrefix = "")
        {
            var type = data.GetType();
            foreach (var field in _mapping.FieldMapping)
            {
                query.Parameters.Add(paramPrefix + field.DbField, type.GetProperty(field.ObjectProperty).GetValue(data, null));
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
                var delStatement = BuildDeleteStatement();
                AppendFilter(delStatement, new string[] { "id" });

                var cmd = new Query();
                cmd.Text = delStatement.ToString();
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
                    SetNamedParameters(cmd, item);
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
                var update = BuildUpdateStatement();
                AppendFilter(update, new string[] { "id" });

                var cmd = new Query();
                cmd.Text = update.ToString();
                SetNamedParameters(cmd, item);
                cmd.Parameters.Add("filterid", item.Key.Value);
                _db.ExecuteCommand(cmd);

                
            }
        }

        public T FindByKey(Identifier key)
        {
            if (!key.IsEmpty())
            {
                var select = BuildSelectStatement();
                AppendFilter(select, new string[] { "id" });

                var cmd = new Query();
                cmd.Text = select.ToString();
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

        #endregion
    }
}
