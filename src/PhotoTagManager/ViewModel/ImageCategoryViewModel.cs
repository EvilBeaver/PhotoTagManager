using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PhotoTagManager.ViewModel
{
    abstract class ImageCategoryViewModel : IconicViewModel
    {
        private ObservableCollection<ImageStreamViewModel> _childItems = null;

        protected virtual void RetrieveChildItems(ICollection<ImageStreamViewModel> destination)
        {
        }

        public ObservableCollection<ImageStreamViewModel> ChildItems
        {
            get
            {
                if (_childItems == null)
                {
                    _childItems = new ObservableCollection<ImageStreamViewModel>();
                    RetrieveChildItems(_childItems);
                }

                return _childItems;
            }
        }

    }
}
