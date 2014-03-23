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
