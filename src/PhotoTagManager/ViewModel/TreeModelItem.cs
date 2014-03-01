using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PhotoTagManager.ViewModel
{
    abstract class TreeModelItem<T> : IconicViewModel
    {
        private bool _isSelected;
        private bool _hasChildren;
        private TreeModelItem<T> _parent;
        private ObservableCollection<TreeModelItem<T>> _children;

        public TreeModelItem(TreeModelItem<T> parent)
        {
            _parent = parent;
        }

        public bool IsSelected 
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool HasChildren
        {
            get
            {
                return _hasChildren;
            }
            protected set
            {
                _hasChildren = value;
                OnPropertyChanged("HasChildren");
            }
        }

        public TreeModelItem<T> Parent
        {
            get
            {
                return _parent;
            }
        }

        public ObservableCollection<TreeModelItem<T>> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<TreeModelItem<T>>();
                    FillChildren(_children);
                }

                return _children;
            }
        }

        abstract protected void FillChildren(ICollection<TreeModelItem<T>> children);
        
    }
}
