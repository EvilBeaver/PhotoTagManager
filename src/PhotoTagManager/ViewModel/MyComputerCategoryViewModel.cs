using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class MyComputerCategoryViewModel : ImageCategoryViewModel
    {
        IImageStreamHost _model;
        
        public MyComputerCategoryViewModel(IImageStreamHost model)
	    {
            _model = model;
	    }

        public static MyComputerCategoryViewModel Create()
        {
            var model = new MyComputerImageCategory();
            return new MyComputerCategoryViewModel(model);
        }


        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return Lib.WinAPI.IconExtractor.GetSpecialFolderIcon(Environment.SpecialFolder.MyComputer, Lib.WinAPI.IconExtractor.IconSize.Small, Lib.WinAPI.IconExtractor.FolderType.Closed);
        }

        protected override string GetHeader()
        {
            return "My Computer";
        }

        protected override void RetrieveChildItems(ICollection<ImageStreamViewModel> destination)
        {
            foreach (var item in _model.ChildItems)
            {
                var dirModel = new DirectoryImageStreamViewModel((DirectoryImageStream)item);
                destination.Add(dirModel);
            }
        }
    }
}
