using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoTagManager
{
    class BitmapImageThumbnailProvider : IThumbnailProvider
    {

        #region IThumbnailProvider Members

        public ImageSource GetThumbnail(string imagePath)
        {
            return GetThumbnail(imagePath, ThumbnailQuality.Normal);
        }

        public ImageSource GetThumbnail(string imagePath, ThumbnailQuality quality)
        {
            return GetThumbnailInternal(imagePath, TranslateQualitySetting(quality));
        }

        private int TranslateQualitySetting(ThumbnailQuality quality)
        {
            switch (quality)
            {
                case ThumbnailQuality.Normal:
                    return 200;
                case ThumbnailQuality.High:
                    return 400;
                case ThumbnailQuality.Full:
                    return 0;
                default:
                    throw new ArgumentException();
            }
        }

        private static ImageSource GetThumbnailInternal(string imagePath, int compression)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            if (compression != 0)         
            {
                bi.DecodePixelWidth = compression; 
            }
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(imagePath);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        #endregion
    }
}
