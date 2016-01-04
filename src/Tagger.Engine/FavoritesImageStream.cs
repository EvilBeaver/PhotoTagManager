using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    class FavoritesImageStream : IImageStreamHost, IImageStream
    {
        private IList<ImageInfo> _images;
        private IList<IHostableImageStream> _childItems;

        public void AddToFavorites(IImageStream stream)
        {
            // TODO:
            // make an autopersistable stream feature

            var dirStream = stream as DirectoryImageStream;
            if (dirStream != null)
            {
                var db = DAL.DatabaseService.GetInstance();
                using (var transaction = db.BeginTransaction())
                {
                    var folderRef = new DAL.FolderRefEntity();
                    folderRef.Path = dirStream.Path;

                    DAL.DatabaseService.FolderRefRepository.Write(folderRef);

                    var fav_item = new DAL.FavoritesStreamReference();
                    fav_item.id = folderRef.Key;
                    fav_item.TableName = "folder_refs";

                    DAL.DatabaseService.FavoritesRepository.Write(fav_item);

                    db.CommitTransaction();

                }

            }

        }

        #region IImageStream Members

        public string Name
        {
            get
            {
                return "Favorites";
            }
            set
            {
                
            }
        }

        public IList<ImageInfo> Images
        {
            get 
            {
                if (_images == null)
                {
                    _images = new List<ImageInfo>();

                }

                return _images;
            }
        }

        #endregion

        #region IImageStreamHost Members

        public IList<IHostableImageStream> ChildItems
        {
            get { return null; }
        }

        #endregion
    }
}
