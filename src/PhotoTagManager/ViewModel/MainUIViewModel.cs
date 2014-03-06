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
        private ObservableCollection<ImageCategoryViewModel> _streams = new ObservableCollection<ImageCategoryViewModel>();
        
        public MainUIViewModel()
        {
            _streams.Add(MyComputerCategoryViewModel.Create());
        }

        public ObservableCollection<ImageCategoryViewModel> Categories
        {
            get { return _streams; }
        }
    }
}
