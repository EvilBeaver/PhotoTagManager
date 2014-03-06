using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PhotoTagManager.Lib.MVVM;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class ImageStreamCategoryViewModel : IconicViewModel
    {
        private ImageStreamCategory _model;
        private ImageSource _icon;
        private ObservableCollection<ImageStreamViewModel> _streams = new ObservableCollection<ImageStreamViewModel>();

        public ImageStreamCategoryViewModel(ImageStreamCategory model)
        {
            _model = model;
            foreach (var item in _model.ChildItems)
            {
                _streams.Add(new ImageStreamViewModel(item));
            }
        }

        public override ImageSource Icon 
        { 
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public override string Header
        {
            get
            {
                return _model.Name;
            }
            set
            {
                _model.Name = value;
                OnPropertyChanged("Header");
            }
        }

        public ObservableCollection<ImageStreamViewModel> Items
        {
            get
            {
                return _streams;
            }
        }

    }
}
