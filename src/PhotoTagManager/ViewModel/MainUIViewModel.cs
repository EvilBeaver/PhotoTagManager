using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class MainUIViewModel : Lib.MVVM.ViewModelBase
    {
        private ImageStreamViewModel _currentImageStream;
        private ObservableCollection<ImageCategoryViewModel> _streams = new ObservableCollection<ImageCategoryViewModel>();
        
        public MainUIViewModel()
        {
            _streams.Add(MyComputerCategoryViewModel.Create());
            _streams.Add(FavoritesCategoryViewModel.Create());
        }

        public ObservableCollection<ImageCategoryViewModel> Categories
        {
            get { return _streams; }
        }

        public ImageStreamViewModel CurrentImageStream
        {
            get
            {
                return _currentImageStream;
            }
            set
            {
                _currentImageStream = value;
                OnPropertyChanged("CurrentImageStream");
            }
        }
    }
}
