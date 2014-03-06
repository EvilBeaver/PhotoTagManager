using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PhotoTagManager.ViewModel
{
    abstract class IconicViewModel : Lib.MVVM.ViewModelBase
    {
        private ImageSource _icon;
        private string _header;

        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = GetIcon();
                }
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public string Header
        {
            get
            {
                if (_header == null)
                {
                    _header = GetHeader();
                }
                return _header;
            }
            set
            {
                _header = value;
                OnPropertyChanged("Header");
            }
        }

        abstract protected ImageSource GetIcon();
        abstract protected string GetHeader();

    }
}
