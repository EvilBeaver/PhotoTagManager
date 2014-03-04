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
        private ObservableCollection<ImageStreamViewModel> _streams = new ObservableCollection<ImageStreamViewModel>();
        
        public MainUIViewModel()
        {
            _streams.Add(new MyComputerHomeStream());
        }

        public ObservableCollection<ImageStreamViewModel> Streams
        {
            get { return _streams; }
        }
    }
}
