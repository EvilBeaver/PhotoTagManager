using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    abstract class NavigationItem : IconicViewModel
    {
        private bool _isSelected;
        private bool _isExpanded;
        
        public bool IsRootItem { get; protected set; }

        public virtual ObservableCollection<ImageInfo> Images
        {
            get { return null; }
        }

        public virtual ObservableCollection<NavigationItem> ChildItems
        {
            get { return null; }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                OnSelectedChange(_isSelected, ref value);
                _isSelected = value;
                OnPropertyChanged("IsSelected");
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
                OnExpandedChange(_isExpanded, ref value);
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        protected virtual void OnSelectedChange(bool oldVal, ref bool newVal)
        {
        }

        protected virtual void OnExpandedChange(bool oldVal, ref bool newVal)
        {
        }

    }
}
