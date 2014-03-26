using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL.Abstract
{
    public class Query
    {
        private Dictionary<string, object> _paramsCollection = new Dictionary<string, object>();

        public string Text { get; set; }

        public IDictionary<string, object> Parameters
        {
            get
            {
                return _paramsCollection;
            }
        }

    }

}
