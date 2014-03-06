using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class DirectoryImageStreamViewModel : ImageStreamViewModel
    {
        private DirectoryImageStream _model;
        private ObservableCollection<ImageStreamViewModel> _childDirectories;

        public DirectoryImageStreamViewModel(DirectoryImageStream model) : base(model)
        {
            _model = model;
        }

        protected override System.Windows.Media.ImageSource GetIcon()
        {
            var icon = Lib.WinAPI.IconExtractor.GetFolderIcon(_model.Path, Lib.WinAPI.IconExtractor.IconSize.Small, Lib.WinAPI.IconExtractor.FolderType.Closed);
            return icon;
        }

        protected override void RetrieveImages(ICollection<ImageInfo> destination)
        {
            foreach (var item in _model.Images)
            {
                destination.Add(item);
            }
        }

        public override System.Collections.ObjectModel.ObservableCollection<ImageStreamViewModel> ChildItems
        {
            get
            {
                if (_childDirectories == null)
                {
                    _childDirectories = new ObservableCollection<ImageStreamViewModel>(
                        _model.ChildItems.Select(x => new DirectoryImageStreamViewModel((DirectoryImageStream)x)));
                }

                return _childDirectories;
            }
        }
    }
}
