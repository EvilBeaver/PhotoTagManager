using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    class TableMapping
    {
        private List<FieldMapping> _map = new List<FieldMapping>();

        public string TableName { get; set; }
        public List<FieldMapping> FieldMapping
        {
            get
            {
                return _map;
            }
        }

        public FieldMapping AddFieldFor(string property, SimpleFieldType type, int len)
        {
            var map = new FieldMapping()
            {
                DbField = property,
                ObjectProperty = property,
                Length = len,
                Type = type
            };

            FieldMapping.Add(map);
            
            return map;
        }
        
    }

    class FieldMapping
    {
        public string DbField;
        public string ObjectProperty;
        public int Length;
        public SimpleFieldType Type;
        public FieldIndex Indexed;
    }

    enum SimpleFieldType
    {
        String,
        Integer,
        Double,
        Date
    }

    enum FieldIndex
    {
        None,
        NotUnique,
        Unique,
        PrimaryKey
    }
}
