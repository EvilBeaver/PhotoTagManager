using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class DirectoryImageStream : IHostableImageStream, IImageStreamHost
    {
        private string _path;
        private string _pattern;
        private IImageStreamHost _parent;
        private List<ImageInfo> _images;
        private List<IHostableImageStream> _childDirectories;

        private object _enumLock = new object();

        public DirectoryImageStream(string path, string pattern, IImageStreamHost parent)
        {
            _path = path;
            _pattern = pattern;
            _parent = parent;
            Name = System.IO.Path.GetFileName(_path);
            if (Name == "")
            {
                Name = _path;
            }
        }

        public DirectoryImageStream(string path, string pattern) : this(path, pattern, null) { }
        
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

        #region IHostableImageStream Members

        public IImageStreamHost Parent
        {
            get { return _parent; }
        }

        #endregion

        #region IImageStreamHost Members

        public IList<IHostableImageStream> ChildItems
        {
            get 
            {
                if (_childDirectories == null)
                {
                    _childDirectories = Directory.EnumerateDirectories(this.Path)
                        .Where(x => TestDirIsHidden(x))
                        .OrderBy(x => x)
                        .Select(x => (IHostableImageStream)new DirectoryImageStream(x, _pattern, this))
                        .ToList();                        
                }

                return _childDirectories;
            }
        }

        private static bool TestDirIsHidden(string dir)
        {
            try
            {
                return !File.GetAttributes(dir).HasFlag(FileAttributes.Hidden);
            }
            catch
            {
#if DEBUG
                throw;
#else
                return false;
#endif
            }
        }

        #endregion
    }
}
