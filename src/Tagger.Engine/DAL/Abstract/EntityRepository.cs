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
        
        public EntityRepository(TableMapping mapping)
        {
            _mapping = mapping;
            CreateTableIfNeeded();
        }

        private void CreateTableIfNeeded()
        {
            using (var con = DatabaseService.GetInstance().OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'",
                        _mapping.TableName);
                    var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    if (!reader.HasRows)
                    {
                        CreateTable(con);
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
                
                sb.AppendLine(field.DbField);
                isFirst = false;
            }

            sb.AppendFormat("FROM {0}", _mapping.TableName);

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
            sb.AppendFormat("INSERT INTO {0}\n", _mapping.TableName);
            sb.AppendLine("VALUES");
            bool isFirst = true;
            foreach (var field in _mapping.FieldMapping)
            {
                if (!isFirst)
                {
                    sb.Append(",\n");
                }
                sb.AppendFormat("({0} = @{0})", field.DbField);
            }

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

            sb.AppendLine("WHERE");

            for (int i = 0; i < filterColumns.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append("\nAND ");
                }

                sb.AppendFormat("{0} = @filter{0}", filterColumns[i]);
            }
        }

        private void SetNamedParameters(SQLiteCommand cmd, object data, string paramPrefix = "")
        {
            var type = data.GetType();
            foreach (var field in _mapping.FieldMapping)
            {
                cmd.Parameters.AddWithValue(paramPrefix+field.DbField, type.GetProperty(field.ObjectProperty).GetValue(data, null));
            }
        }

        abstract protected T NewInstance();
        
        private void Hydrate(T instance, SQLiteDataReader reader)
        {
            OnHydrate(instance, reader);
        }

        protected virtual void OnHydrate(T instance, SQLiteDataReader reader)
        {
            var id = new Identifier()
            {
                Value = (int)reader["id"]
            };

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
                using (var con = DatabaseService.GetInstance().OpenConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = delStatement.ToString();
                        cmd.Parameters.AddWithValue("filterid", item.Key.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Write(T item)
        {
            if (item.Key.IsEmpty())
            {
                var insert = BuildInsertStatement();
                using (var con = DatabaseService.GetInstance().OpenConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = insert;
                        SetNamedParameters(cmd, item);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = String.Format("SELECT last_insert_rowid() FROM [{0}]", _mapping.TableName);
                        cmd.Parameters.Clear();
                        var rowId = (int)cmd.ExecuteScalar();
                        item.Key = new Identifier() { Value = rowId };
                    }
                }
            }
            else
            {
                var update = BuildUpdateStatement();
                AppendFilter(update, new string[] { "id" });
                using (var con = DatabaseService.GetInstance().OpenConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = update.ToString();
                        SetNamedParameters(cmd, item);
                        cmd.Parameters.AddWithValue("filterid", item.Key.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public T FindByKey(Identifier key)
        {
            if (!key.IsEmpty())
            {
                var select = BuildSelectStatement();
                AppendFilter(select, new string[] { "id" });
                using (var con = DatabaseService.GetInstance().OpenConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("filterid", key.Value);
                        var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                        if (reader.HasRows)
                        {
                            var item = NewInstance();
                            Hydrate(item, reader);
                            return item;
                        }
                        else
                        {
                            return default(T);
                        }
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
            using (var con = DatabaseService.GetInstance().OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    var reader = cmd.ExecuteReader();
                    var item = NewInstance();
                    Hydrate(item, reader);
                    list.Add(item);
                }
            }

            return list;

        }

        #endregion
    }
}
