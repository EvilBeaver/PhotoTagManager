using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        public bool IsRootStream { get; set; }
    }

    class TreeViewFirstItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var element = container as TreeViewItem;
            if (element != null && item is ImageStreamViewModel)
            {
                var model = (ImageStreamViewModel)item;
                if (model.IsRootStream)
                {
                    return element.FindResource("StreamHeaderStyle") as Style;
                }
                
            }
            return null;
        }
    }

}
