using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoTagManager.Lib.WinAPI;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class FileSystemViewModel : ImageStreamViewModel
    {
        private DirectoryImageStream _model;
        private const string PATTERN = "*.jpg";

        public static FileSystemViewModel Create(string root, ImageStreamViewModel parent)
        {
            var model = new DirectoryImageStream(root, PATTERN);
            return new FileSystemViewModel(model, parent);
        }

        private FileSystemViewModel(DirectoryImageStream model, ImageStreamViewModel parent)
            : base(model, parent)
        {
            _model = model;
            if (parent == null)
            {
                IsRootStream = true;
            }

            Header = _model.Name;
            Icon = IconExtractor.GetFolderIcon(_model.Path, IconExtractor.IconSize.Small, IconExtractor.FolderType.Closed);
        }

        protected override void FillChildren(ICollection<TreeModelItem<ImageStreamViewModel>> children)
        {
            try
            {
                var di = new System.IO.DirectoryInfo(_model.Path);
                var dirs = di.EnumerateDirectories()
                    .Where(x => !x.Attributes.HasFlag(System.IO.FileAttributes.Hidden))
                    .OrderBy(x=>x.Name)
                    .Select(x=>x.FullName);

                
                foreach (var dir in dirs)
                {
                    var model = new DirectoryImageStream(dir, PATTERN);
                    var childViewModel = new FileSystemViewModel(model, this);
                    children.Add(childViewModel);

                }

                
            }
            catch
            {
            }
        }
    }
}
