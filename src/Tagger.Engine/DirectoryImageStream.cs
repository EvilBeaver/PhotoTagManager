using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class DirectoryImageStream : IImageStream
    {
        private string _path;
        private string _pattern;
        private List<ImageInfo> _images;
        private object _enumLock = new object();

        public DirectoryImageStream(string path, string pattern)
        {
            _path = path;
            _pattern = pattern;
            Name = System.IO.Path.GetFileName(_path);
        }
        
        #region IImageStream Members

        public string Name { get; set; }

        public string Path { get { return _path; } }
        
        public IList<ImageInfo> Images
        {
            get 
            {
                if (_images == null)
                {
                    lock (_enumLock)
                    {
                        if (_images == null)
                        {
                            _images = new List<ImageInfo>();
                            var scanner = new FileScanner();
                            var fileInfos = scanner.ScanFolder(_path, _pattern);
                            foreach (var item in fileInfos)
                            {
                                _images.Add(new ImageInfo()
                                    {
                                        File = item
                                    });
                            }
                        }
                    }
                }

                return _images;
            }
        }

        #endregion
    }
}
