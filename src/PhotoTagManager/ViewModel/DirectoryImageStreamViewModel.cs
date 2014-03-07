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
        private ObservableCollection<ImageStreamViewModel> _childDirectories = new ObservableCollection<ImageStreamViewModel>();
        private bool _hasDummyChild;
        private bool _isExpanded;

        public DirectoryImageStreamViewModel(DirectoryImageStream model) : base(model)
        {
            _model = model;
            _childDirectories.Add(new DummyImageStreamViewModel());
            _hasDummyChild = true;
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
                return _childDirectories;
            }
        }

        private void FetchRealData()
        {
            if (_hasDummyChild && _isExpanded)
            {
                _childDirectories.Clear();
                var dirs = new ObservableCollection<ImageStreamViewModel>(
                    _model.ChildItems.Select(x => new DirectoryImageStreamViewModel((DirectoryImageStream)x)));

                foreach (var dir in dirs)
                {
                    _childDirectories.Add(dir);
                }

                _hasDummyChild = false;
            }
        }

        public bool IsExpanded 
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                if (_isExpanded && _hasDummyChild)
                {
                    FetchRealData();
                }
                OnPropertyChanged("IsExpanded");
            }
        }

    }
}
