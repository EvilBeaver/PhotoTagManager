using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL.Abstract;

namespace Tagger.Engine.DAL
{
    public class FavoritesRepository : RegistryRepository<FavoritesStreamReference>
    {
        private FavoritesRepository(IDatabase db, TableMapping mapping) : base(db, mapping)
        {
        }

        protected override FavoritesStreamReference NewInstance()
        {
            return new FavoritesStreamReference();
        }

        protected override void OnHydrate(ref FavoritesStreamReference instance, System.Data.SQLite.SQLiteDataReader reader)
        {
            var id = new Identifier(reader["table_id"]);
            
            instance.id = id;
            instance.TableName = (string)reader["table"];

        }

        public static FavoritesRepository Create()
        {
            return new FavoritesRepository(DatabaseService.GetInstance(), GetMapping());
        }

        public static FavoritesRepository Create(IDatabase db)
        {
            return new FavoritesRepository(db, GetMapping());
        }

        private static TableMapping GetMapping()
        {
            var map = new TableMapping();
            map.TableName = "favorites";

            FieldMapping fieldDescr;

            fieldDescr = new FieldMapping();
            fieldDescr.DbField = "table";
            fieldDescr.ObjectProperty = "TableName";
            fieldDescr.Type = SimpleFieldType.String;
            fieldDescr.Length = 100;
            fieldDescr.Indexed = FieldIndex.PrimaryKey;
            map.FieldMapping.Add(fieldDescr);

            fieldDescr = new FieldMapping();
            fieldDescr.DbField = "table_id";
            fieldDescr.ObjectProperty = "id";
            fieldDescr.Type = SimpleFieldType.Integer;
            fieldDescr.Indexed = FieldIndex.PrimaryKey;
            map.FieldMapping.Add(fieldDescr);

            return map;
        }

    }

    public struct FavoritesStreamReference
    {
        public string TableName { get; set; }
        public Identifier id { get; set; } 
    }
}
