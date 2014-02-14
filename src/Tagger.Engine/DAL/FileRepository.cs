using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    class FileRepository : RepositoryBase<FileLink>
    {
   
        protected override void InternalAdd(IPersistable<FileLink> item)
        {
            var sb = new StringBuilder();
            var data = item.Value;
            sb.AppendLine("INSERT INTO [files]");
            sb.AppendLine("([name],[fullname],[md5])");
            sb.AppendLine("VALUES(?,?,?)");

            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("name", data.Name);
                    cmd.Parameters.AddWithValue("fullname", data.FullName);
                    cmd.Parameters.AddWithValue("md5", data.MD5);
                    cmd.ExecuteNonQuery();
                }

            }
        }

        protected override void InternalRemove(IPersistable<FileLink> item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM [files] WHERE [id] = ?";
                    cmd.Parameters.AddWithValue("id", item.Key.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void InternalWrite(IPersistable<FileLink> item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = 
                    @"UPDATE [files] SET 
                        [name] = ?,
                        [fullname] = ?,
                        [md5] = ? 
                    WHERE [id]=?";

                    cmd.Parameters.AddWithValue("name", item.Value.Name);
                    cmd.Parameters.AddWithValue("fullname", item.Value.FullName);
                    cmd.Parameters.AddWithValue("md5", item.Value.MD5);
                    cmd.Parameters.AddWithValue("id", item.Key.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override IPersistable<FileLink> FindByKey(Identifier key)
        {
            if (key.IsEmpty())
            {
                return null;
            }

            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT [name],[fullname],[md5],[id] FROM [files] WHERE [id] = ?";
                    cmd.Parameters.AddWithValue("id", key.Value);
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                return Hydrate(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }

        }

        public IPersistable<FileLink> FindByFullPath(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT [name],[fullname],[md5],[id] FROM [files] WHERE [fullname] = ?";
                    cmd.Parameters.AddWithValue("fullname", path);
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                return Hydrate(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }

        }


        private static IPersistable<FileLink> Hydrate(DbDataReader data)
        {
            var link = FileLink.CreateEmpty();
            link.Name = data.GetString(0);
            link.FullName = data.GetString(1);
            link.MD5 = data.GetString(2);

            var persistable = FileLinkPersistor.Create(link);
            persistable.Key = new Identifier() { Value = data.GetInt32(3) };

            return persistable;
        }

        public override IEnumerable<IPersistable<FileLink>> GetAll()
        {
            List<IPersistable<FileLink>> result = new List<IPersistable<FileLink>>();
            
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT [name],[fullname],[md5],[id] FROM [files]";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(Hydrate(reader));
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<IPersistable<FileLink>> GetByPathBase(string pathBase)
        {
            return null;
        }
    }

    class FileLinkPersistor : IPersistable<FileLink>
    {
        private Identifier _id;
        private FileLink _data;

        private FileLinkPersistor(FileLink data)
        {
            _data = data;
        }

        #region IPersistable<FileLink> Members

        public Identifier Key
        {
            get 
            {
                return _id; 
            }
            internal set
            {
                _id = value;
            }
        }

        public FileLink Value
        {
            get { return _data; }
        }

        #endregion

        public override string ToString()
        {
            return Value.ToString() + "#" + Key.Value.ToString();
        }

        public static FileLinkPersistor Create(FileLink data)
        {
            return new FileLinkPersistor(data);
        }
    }
}
