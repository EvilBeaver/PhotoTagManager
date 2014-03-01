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
            var imagesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var root = FileSystemViewModel.Create(imagesPath);
            _streams.Add(root);
        }

        public ObservableCollection<ImageStreamViewModel> Streams
        {
            get { return _streams; }
        }
    }
}
