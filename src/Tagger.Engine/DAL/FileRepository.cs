using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    class FileRepository : RepositoryBase<FileLink>
    {
   
        protected override void InternalAdd(FileLink item)
        {
            var sb = new StringBuilder();
            var data = item;
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

        protected override void InternalRemove(FileLink item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM [files] WHERE [id] = ?";
                    cmd.Parameters.AddWithValue("id", item.AsPersistable().Key.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void InternalWrite(FileLink item)
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

                    cmd.Parameters.AddWithValue("name", item.Name);
                    cmd.Parameters.AddWithValue("fullname", item.FullName);
                    cmd.Parameters.AddWithValue("md5", item.MD5);
                    cmd.Parameters.AddWithValue("id", item.AsPersistable().Key.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override FileLink FindByKey(Identifier key)
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

        public FileLink FindByFullPath(string path)
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


        private static FileLink Hydrate(DbDataReader data)
        {
            var link = FileLink.CreateEmpty();
            link.Name = data.GetString(0);
            link.FullName = data.GetString(1);
            link.MD5 = data.GetString(2);
            link.AsPersistable().Key = new Identifier() { Value = data.GetInt32(3) };

            return link;

        }

        public override IEnumerable<FileLink> GetAll()
        {
            List<FileLink> result = new List<FileLink>();
            
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

        public IEnumerable<FileLink> GetByPathBase(string pathBase)
        {
            return null;
        }
    }

    
}
