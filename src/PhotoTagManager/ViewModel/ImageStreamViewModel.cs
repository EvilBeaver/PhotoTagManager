using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
}
