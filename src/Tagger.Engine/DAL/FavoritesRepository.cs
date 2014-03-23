using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    class FavoritesRepository : RepositoryBase<IPersistableImageStream>
    {

        protected override void InternalAdd(IPersistableImageStream item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    var qry = "INSERT INTO [favorites] VALUES (?,?)";
                    cmd.Parameters.AddWithValue("table", item.TableName);
                    cmd.Parameters.AddWithValue("ref", item.Key.Value);
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void InternalRemove(IPersistableImageStream item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    var qry = "DELETE FROM [favorites] WHERE [table] = ? AND [table_id] = ?";
                    cmd.Parameters.AddWithValue("table", item.TableName);
                    cmd.Parameters.AddWithValue("ref", item.Key.Value);
                    cmd.CommandText = qry;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void InternalWrite(IPersistableImageStream item)
        {
            
        }
    }

    class FolderRefRepository : RepositoryBase<DirectoryPersistableRef>
    {
        protected override void InternalAdd(DirectoryPersistableRef item)
        {
            using (var con = Database.OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [folder_refs] VALUES (?)";
                    cmd.Parameters.AddWithValue("path", item.ImageStream.Path);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT last_insert_rowid() FROM [folder_refs]";
                    int id = (int)cmd.ExecuteScalar();
                    item.Key = new Identifier() { Value = id };
                }
            }
        }

        protected override void InternalRemove(DirectoryPersistableRef item)
        {
            throw new NotImplementedException();
        }

        protected override void InternalWrite(DirectoryPersistableRef item)
        {
            throw new NotImplementedException();
        }
    }
}
