using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PhotoTagManager
{
    interface IThumbnailProvider
    {
        ImageSource GetThumbnail(string imagePath);
        ImageSource GetThumbnail(string imagePath, ThumbnailQuality quality);
    }

    enum ThumbnailQuality
    {
        Normal,
        High,
        Full
    }

}
