using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine.DAL;

namespace Tagger.Engine
{
    public class FileLink : IPersistable
    {
        private string _md5;
        private object _lock = new object();
        private Identifier _databaseId;

        private FileLink()
        {
        }

        private FileLink(string path)
        {
            FullName = path;
            Name = System.IO.Path.GetFileName(path);
        }

        private string CalculateHash()
        {
            if (System.IO.File.Exists(FullName))
            {
                using (var stream = new System.IO.FileStream(FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (var hasher = System.Security.Cryptography.MD5.Create())
                {
                    var hashArr = hasher.ComputeHash(stream);
                    var sb = new StringBuilder();
                    for (int i = 0; i < hashArr.Length; i++)
                    {
                        sb.AppendFormat("{0:x}", hashArr[i]);
                    }
                    return sb.ToString();
                }
            }
            else
            {
                return "";
            }
        }

        public string FullName { get; internal set; }
        public string Name { get; internal set; }
        public string MD5 
        {
            get
            {
                if (_md5 == null)
                {
                    lock (_lock)
                    {
                        if (_md5 == null)
                        {
                            _md5 = CalculateHash();
                        }
                    }
                }
                
                return _md5;
            }
            internal set
            {
                lock (_lock)
                {
                    _md5 = value;
                }
            }
        }

        public void RecalculateMD5()
        {
            lock (_lock)
            {
                _md5 = CalculateHash();
            }
        }

        public static FileLink Create(string path)
        {
            return new FileLink(path);
        }

        public static FileLink CreateEmpty()
        {
            return new FileLink();
        }

        public override string ToString()
        {
            return FullName;
        }


        #region IPersistable<FileLink> Members

        Identifier IPersistable.Key
        {
            get { return _databaseId; }
            set { _databaseId = value; }
        }

        internal IPersistable AsPersistable()
        {
            return (IPersistable)this;
        }

        #endregion
    }
}
