using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class DirectoryStreamViewModel : NavigationItem
    {
        private DirectoryImageStream _model;
        private bool _hasDummyChild;
        private ObservableCollection<NavigationItem> _childNodes;
        private ObservableCollection<ImageInfo> _images;
        
        private static DummyNavigationNode _staticDummyNode = new DummyNavigationNode();

        public DirectoryStreamViewModel(DirectoryImageStream model)
        {
            _model = model;
            _childNodes = new ObservableCollection<NavigationItem>();
            _childNodes.Add(_staticDummyNode);
            _hasDummyChild = true;
        }
        
        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return Lib.WinAPI.IconExtractor.GetFolderIcon(_model.Path, 
                Lib.WinAPI.IconExtractor.IconSize.Small, 
                Lib.WinAPI.IconExtractor.FolderType.Closed);
        }

        protected override string GetHeader()
        {
            return _model.Name;
        }

        public override System.Collections.ObjectModel.ObservableCollection<NavigationItem> ChildItems
        {
            get
            {
                return _childNodes;
            }
        }

        protected override void OnExpandedChange(bool oldVal, ref bool newVal)
        {
            if (newVal == true && _hasDummyChild)
            {
                _childNodes.Clear();
                foreach (var item in _model.ChildItems)
                {
                    _childNodes.Add(new DirectoryStreamViewModel((DirectoryImageStream)item));
                }
                _hasDummyChild = false;
            }
        }

        public override ObservableCollection<ImageInfo> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<ImageInfo>();
                    foreach (var item in _model.Images)
                    {
                        _images.Add(item);
                    }
                }

                return _images;
            }
        }
    }

    class DummyNavigationNode : NavigationItem
    {
        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return null;
        }

        protected override string GetHeader()
        {
            return "loading...";
        }
    }
}
