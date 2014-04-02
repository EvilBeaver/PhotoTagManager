using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL.Abstract;

namespace Tagger.Engine.DAL
{
    class FolderRefRepository : EntityRepository<FolderRefEntity>
    {
        private FolderRefRepository(IDatabase db, TableMapping mapping) : base(db, mapping)
        {
        }

        protected override FolderRefEntity NewInstance()
        {
            return new FolderRefEntity();
        }

        public static FolderRefRepository Create()
        {
            return new FolderRefRepository(DatabaseService.GetInstance(), GetFieldMap());
        }

        public static FolderRefRepository Create(IDatabase db)
        {
            return new FolderRefRepository(db, GetFieldMap());
        }

        private static TableMapping GetFieldMap()
        {
            TableMapping map = new TableMapping();
            FieldMapping fieldDescr;

            map.TableName = "folder_refs";
            fieldDescr = map.AddFieldFor("Path", SimpleFieldType.String, 260);
            fieldDescr.DbField = "path";

            return map;
        }

    }

    class FolderRefEntity : IPersistable
    {
        public string Path { get; set; }
        
        #region IPersistable Members

        public Identifier Key
        {
            get;
            set;
        }

        #endregion
    }

}
