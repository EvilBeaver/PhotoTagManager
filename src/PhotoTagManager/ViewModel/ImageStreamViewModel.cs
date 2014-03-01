using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    abstract class ImageStreamViewModel : TreeModelItem<ImageStreamViewModel>
    {
        private IImageStream _model;
        
        public ImageStreamViewModel(IImageStream source, ImageStreamViewModel parent) : base(parent)
        {
            _model = source;
        }

        IImageStream ImageStream
        {
            get
            {
                return _model;
            }
        }
    }
}
