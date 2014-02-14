using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class FileLink
    {
        private Lazy<string> _md5;
        private object _lock = new object();

        private FileLink()
        {
        }

        private FileLink(string path)
        {
            FullName = path;
            Name = System.IO.Path.GetFileName(path);

            _md5 = new Lazy<string>(() =>
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
                    return null;
                }
            });

        }

        public string FullName { get; internal set; }
        public string Name { get; internal set; }
        public string MD5 
        {
            get
            {
                return _md5.Value;
            }
            internal set
            {
                string valCopy = value;
                _md5 = new Lazy<string>(()=>valCopy);
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
    }
}
