using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    abstract public class ImageStreamCategory : IImageStreamHost
    {
        private List<IHostableImageStream> _hostedItems = null;

        public string Name { get; set; }

        #region IImageStreamHost Members

        public IList<IHostableImageStream> ChildItems
        {
            get 
            {
                if (_hostedItems == null)
                {
                    FetchChildItems();
                }

                return _hostedItems;
            }
        }

        public void FetchChildItems()
        {
            if (_hostedItems == null)
            {
                var collection = new List<IHostableImageStream>();
                LoadItems(collection);
                _hostedItems = collection;
            }
            
        }

        protected virtual void LoadItems(ICollection<IHostableImageStream> hostedItems)
        {
        }

        #endregion
    }

    public class MyComputerCategory : ImageStreamCategory
    {
        public MyComputerCategory()
        {
            Name = "My Computer";
        }

        protected override void LoadItems(ICollection<IHostableImageStream> hostedItems)
        {
            var drives = System.IO.DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    try
                    {
                        var driveModel = new DirectoryImageStream(drive.Name, "*.jpg", null);
                        hostedItems.Add(driveModel);
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            }
        }
    }
}
