using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger.Engine;

namespace PhotoTagManager.ViewModel
{
    class FavoritesCategoryViewModel : ImageCategoryViewModel
    {
        protected override System.Windows.Media.ImageSource GetIcon()
        {
            return Lib.WinAPI.IconExtractor.GetSpecialFolderIcon(Environment.SpecialFolder.Favorites, 
                Lib.WinAPI.IconExtractor.IconSize.Small,
                Lib.WinAPI.IconExtractor.FolderType.Closed);
        }

        protected override string GetHeader()
        {
            return "Favorites";
        }

        protected override void RetrieveChildItems(ICollection<ImageStreamViewModel> destination)
        {
            var picFolder = new DirectoryImageStream(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "*.jpg");
            var picViewModel = new DirectoryImageStreamViewModel(picFolder);
            destination.Add(picViewModel);
        }

        public static FavoritesCategoryViewModel Create()
        {
            return new FavoritesCategoryViewModel();
        }

    }
}
