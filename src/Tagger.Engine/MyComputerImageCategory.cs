using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tagger.Engine
{
    public class MyComputerImageCategory : IImageStreamHost
    {
        private const string PATTERN = "*.jpg";

        private List<IHostableImageStream> _driveModels;
        
        #region IImageStreamHost Members

        public IList<IHostableImageStream> ChildItems
        {
            get 
            {
                if (_driveModels == null)
                {
                    _driveModels = new List<IHostableImageStream>();
                    var drives = DriveInfo.GetDrives();
                    foreach (var drive in drives)
                    {
                        if (drive.IsReady)
                        {
                            var dirModel = new DirectoryImageStream(drive.Name, PATTERN, this);
                            var sb = new StringBuilder(drive.Name);
                            if(!String.IsNullOrWhiteSpace(drive.VolumeLabel))
                            {
                                sb.AppendFormat(" [{0}]", drive.VolumeLabel);
                            }
                            dirModel.Name = sb.ToString();

                            _driveModels.Add(dirModel);
                        }
                    }
                }

                return _driveModels;
            }
        }

        #endregion
    }
}
