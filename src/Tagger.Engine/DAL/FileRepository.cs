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
        private FileRepository(TableMapping mapping) : base(mapping)
        {

        }

        protected override FileLink NewInstance()
        {
            var instance = FileLink.CreateEmpty();
            return instance;
        }


        public static FileRepository Create()
        {
            TableMapping map = new TableMapping();
            FieldMapping fieldDescr;

            map.TableName = "files";
            map.AddFieldFor("Name", SimpleFieldType.String, 100);
            fieldDescr = map.AddFieldFor("FullName", SimpleFieldType.String, 260);
            fieldDescr.Indexed = FieldIndex.Unique;
            fieldDescr = map.AddFieldFor("MD5", SimpleFieldType.String, 32);
            fieldDescr.Indexed = FieldIndex.NotUnique;

            return new FileRepository(map);
        }

    }

    
}
