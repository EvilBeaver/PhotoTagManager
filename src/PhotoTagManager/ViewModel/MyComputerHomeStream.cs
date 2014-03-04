using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoTagManager.ViewModel
{
    class MyComputerHomeStream : ImageStreamViewModel
    {
        public MyComputerHomeStream() : base(null, null)
        {
           IsRootStream = true;
           this.Header = "Computer";
        }

        protected override void FillChildren(ICollection<TreeModelItem<ImageStreamViewModel>> children)
        {
            foreach (var drive in Environment.GetLogicalDrives())
            {
                var driveModel = FileSystemViewModel.Create(drive, this);
                children.Add(driveModel);
            }
        }
    }
}
