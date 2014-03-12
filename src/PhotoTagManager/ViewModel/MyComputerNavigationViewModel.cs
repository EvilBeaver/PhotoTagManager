using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class MyComputerNavigationViewModel : NavigationItem
    {
        private ObservableCollection<NavigationItem> _drives = new ObservableCollection<NavigationItem>();
        
        public MyComputerNavigationViewModel()
        {
            IsRootItem = true;
            FillDrivesCollection();
        }

        private void FillDrivesCollection()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    var dirModel = new DirectoryImageStream(drive.Name, null);
                    var sb = new StringBuilder(drive.Name);
                    if (!String.IsNullOrWhiteSpace(drive.VolumeLabel))
                    {
                        sb.AppendFormat(" [{0}]", drive.VolumeLabel);
                    }
                    dirModel.Name = sb.ToString();
                    _drives.Add(new DirectoryStreamViewModel(dirModel));
                }
            }
        }

        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return Lib.WinAPI.IconExtractor.GetSpecialFolderIcon(Environment.SpecialFolder.MyComputer,
                Lib.WinAPI.IconExtractor.IconSize.Small,
                Lib.WinAPI.IconExtractor.FolderType.Closed);
        }

        protected override string GetHeader()
        {
            return "My Computer";
        }

        public override ObservableCollection<NavigationItem> ChildItems
        {
            get
            {
                return _drives;
            }
        }
    }
}
