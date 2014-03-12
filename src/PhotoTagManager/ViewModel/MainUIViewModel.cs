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
        private ObservableCollection<NavigationItem> _streams = new ObservableCollection<NavigationItem>();

        public MainUIViewModel()
        {
            _streams.Add(new MyComputerNavigationViewModel());
        }

        public ObservableCollection<NavigationItem> Categories
        {
            get { return _streams; }
        }
    }
}
