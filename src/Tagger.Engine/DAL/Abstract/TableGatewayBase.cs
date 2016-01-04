using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    abstract class TableGatewayBase
    {
        private TableMapping _mapping;

        public TableGatewayBase(TableMapping mapping)
        {
            _mapping = mapping;
        }

        protected TableMapping Mapping
        {
            get { return _mapping; }
        }

        protected void CreateTableIfNeeded(IDatabase db)
        {
            var query = new Query();
            query.Text = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'",
                        _mapping.TableName);
            using (var reader = db.ExecuteReader(query))
            {
                if (!reader.HasRows)
                {
                    var createQry = new Query(BuildCreateTable());
                    db.ExecuteCommand(createQry);
                }
            }
        }

        private string BuildCreateTable()
        {
            var sb = new StringBuilder();
            var indexSb = new StringBuilder();

            sb.AppendFormat("CREATE TABLE {0} (\n", _mapping.TableName);

            var uidFlags = (FieldProperties.AutoIncrement | FieldProperties.PrimaryKey);
            var uid = _mapping.FieldMapping
                .FirstOrDefault(x => (x.PropertyFlags & uidFlags) == uidFlags);

            List<string> fields = new List<string>();
            List<string> indexes = new List<string>();
            List<string> pk = new List<string>();

            if (uid != null)
            {
                fields.Add(string.Format("[{0}] {1} PRIMARY KEY AUTOINCREMENT NOT NULL",
                    uid.DbField, TypeExpression(uid)));
            }

            foreach (var field in _mapping.FieldMapping.Where(x => x != uid))
            {
                fields.Add(string.Format("[{0}] {1} NOT NULL", field.DbField, TypeExpression(field)));
                if (field.PropertyFlags.HasFlag(FieldProperties.Indexed) || field.PropertyFlags.HasFlag(FieldProperties.UniqueIndex))
                {
                    indexes.Add(string.Format("CREATE {0} INDEX [{1}] ON [{2}] ([{3}]);",
                        (field.PropertyFlags & FieldProperties.UniqueIndex) != 0 ? "UNIQUE" : "",
                        "idx_" + field.DbField,
                        _mapping.TableName,
                        field.DbField));
                }

                if (field.PropertyFlags.HasFlag(FieldProperties.PrimaryKey))
                {
                    pk.Add(string.Format("[{0}]", field.DbField));
                }

            }

            sb.AppendLine(JoinExpressions(fields));
            if (pk.Count > 0)
            {
                sb.Append(",\nPRIMARY KEY(\n");
                sb.Append(JoinExpressions(pk));
                sb.Append(')');
            }

            sb.AppendLine("\n);");

            foreach (var idx in indexes)
            {
                sb.AppendLine(idx);
            }

            return sb.ToString();
        }

        protected string BuildInsertStatement()
        {
            var sb = new StringBuilder();

            List<string> fieldList = new List<string>();
            List<string> values = new List<string>();

            foreach (var field in _mapping.FieldMapping)
            {
                if (!field.PropertyFlags.HasFlag(FieldProperties.AutoIncrement))
                {
                    fieldList.Add(string.Format("[{0}]", field.DbField));
                    values.Add(string.Format("@{0}", field.DbField));
                }
            }

            sb.AppendFormat("INSERT INTO [{0}](\n", _mapping.TableName);
            sb.Append(JoinExpressions(fieldList));
            sb.Append(")\nVALUES(\n");
            sb.Append(JoinExpressions(values));
            sb.Append(");");

            return sb.ToString();
        }

        protected string BuildSelectStatement(params string[] filterFields)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT\n");

            string[] expr = new string[_mapping.FieldMapping.Count];

            for (int i = 0; i < expr.Length; i++)
            {
                var field = _mapping.FieldMapping[i];
                expr[i] = string.Format("[{0}]", field.DbField);
            }

            sb.Append(JoinExpressions(expr));
            sb.AppendFormat("\nFROM [{0}]", _mapping.TableName);

            AppendFilter(sb, filterFields);

            return sb.ToString();
        }

        protected string BuildUpdateStatement(params string[] filterFields)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0}\n", _mapping.TableName);

            var fields = new List<string>();

            foreach (var field in _mapping.FieldMapping)
            {
                fields.Add(string.Format("SET {0} = @{0}", field.DbField));
            }

            AppendFilter(sb, filterFields);

            return sb.ToString();
        }

        protected string BuildDeleteStatement(params string[] filterFields)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0}", _mapping.TableName);

            AppendFilter(sb, filterFields);

            return sb.ToString();
        }

        protected void SetQueryParameters(Query cmd, object data)
        {
            SetQueryParameters(cmd, data, "");
        }

        protected void SetQueryParameters(Query cmd, object data, string paramPrefix)
        {
            var type = data.GetType();
            foreach (var field in _mapping.FieldMapping)
            {
                var prop = type.GetProperty(field.ObjectProperty);
                if (prop != null)
                {
                    object paramValue;
                    if (prop.PropertyType == typeof(Identifier))
                    {
                        var id = (Identifier)prop.GetValue(data, null);
                        paramValue = id.Value;
                    }
                    else
                    {
                        paramValue = prop.GetValue(data, null);
                    }

                    cmd.Parameters.Add(paramPrefix + field.DbField, paramValue);
                }

            }
        }

        private void AppendFilter(StringBuilder sb, string[] filterColumns)
        {
            if (filterColumns.Length == 0)
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

                sb.AppendFormat("[{0}] = @filter{0}", filterColumns[i]);
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

        private string JoinExpressions(IEnumerable<string> values)
        {
            return string.Join(",\n", values);
        }

    }
}
