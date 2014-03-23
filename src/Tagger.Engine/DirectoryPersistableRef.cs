using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    class DirectoryPersistableRef : IPersistableImageStream
    {
        private DirectoryImageStream _data;
        private Identifier _id;

        public DirectoryPersistableRef(DirectoryImageStream data)
        {
            _data = data;
        }

        #region IPersistableImageStream Members

        public string TableName
        {
            get
            {
                return "folder_refs";
            }
        }

        public DirectoryImageStream ImageStream
        {
            get
            {
                return _data;
            }
        }

        #endregion

        #region IPersistable Members

        public Identifier Key
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        #endregion
    }
}
