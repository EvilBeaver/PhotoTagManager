using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL.Abstract;

namespace Tagger.Engine.DAL
{
    class FileRepository : EntityRepository<FileLink>
    {
        private FileRepository(IDatabase db, TableMapping mapping) : base(db, mapping)
        {

        }

        protected override FileLink NewInstance()
        {
            var instance = FileLink.CreateEmpty();
            return instance;
        }


        public static FileRepository Create()
        {
            return new FileRepository(DatabaseService.GetInstance(), GetFieldMap());
        }

        public static FileRepository Create(IDatabase db)
        {
            return new FileRepository(db, GetFieldMap());
        }

        private static TableMapping GetFieldMap()
        {
            TableMapping map = new TableMapping();
            FieldMapping fieldDescr;

            map.TableName = "files";
            map.AddFieldFor("Name", SimpleFieldType.String, 100);
            fieldDescr = map.AddFieldFor("FullName", SimpleFieldType.String, 260);
            fieldDescr.PropertyFlags = FieldProperties.UniqueIndex;
            
            fieldDescr = map.AddFieldFor("MD5", SimpleFieldType.String, 32);
            fieldDescr.PropertyFlags = FieldProperties.Indexed;

            return map;
        }

    }

    
}
