using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    abstract class ImageStreamViewModel : IconicViewModel
    {
        IImageStream _model;
        ObservableCollection<ImageInfo> _images;

        public ImageStreamViewModel(IImageStream model)
        {
            _model = model;
        }

        protected virtual void RetrieveImages(ICollection<ImageInfo> destination)
        {
            foreach (var image in _model.Images)
            {
                destination.Add(image);
            }
        }
        
        protected override string GetHeader()
        {
            return _model.Name;
        }

        public ObservableCollection<ImageInfo> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<ImageInfo>();
                    RetrieveImages(_images);
                }

                return _images;
            }
        }

        public virtual ObservableCollection<ImageStreamViewModel> ChildItems
        {
            get
            {
                return null;
            }
        }

    }

    class DummyImageStreamViewModel : ImageStreamViewModel
    {

        public DummyImageStreamViewModel() : base(null)
        {

        }

        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return null;
        }

        protected override string GetHeader()
        {
            return "loading...";
        }
    }

    class CategoryRootStyleSelector : StyleSelector
    {
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var element = container as TreeViewItem;

            if (element != null && item is ImageCategoryViewModel)
            {
                return element.FindResource("StreamHeaderStyle") as System.Windows.Style;
            }

            if (element != null && item is ImageStreamViewModel)
            {
                return element.FindResource("StandartStyle") as System.Windows.Style;
            }

            return null;
        }
    }
}
