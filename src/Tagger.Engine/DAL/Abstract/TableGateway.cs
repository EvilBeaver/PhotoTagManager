using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    class TableGateway
    {
        private TableMapping _mapping;

        public TableGateway(TableMapping mapping)
        {
            _mapping = mapping;
        }

        public string TableName
        {
            get
            {
                return _mapping.TableName;
            }
        }

        public IEnumerable<FieldMapping> Fields 
        {
            get
            {
                return _mapping.FieldMapping;
            }
        }

    }
}
