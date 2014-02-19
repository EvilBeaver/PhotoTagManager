using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tagger.Engine;

namespace PhotoTagManager
{
    /// <summary>
    /// Interaction logic for PhotoView.xaml
    /// </summary>
    public partial class PhotoView : UserControl
    {
        public PhotoView()
        {
            InitializeComponent();
        }

        public void SetItemsSource(IEnumerable<FileLink> source)
        {
            var ctx = SynchronizationContext.Current;

            var items = source.Select((x) => new ListItem(x)).ToList();
            lvItems.ItemsSource = items;

            var thumbnailer = new BitmapImageThumbnailProvider();

            Task.Factory.StartNew(() =>
               {
                   Parallel.ForEach(Partitioner.Create(0, items.Count), (range) =>
                       {
                           for (int i = range.Item1; i < range.Item2; i++)
                           {
                               var currentItem = items[i];
                               var tb = thumbnailer.GetThumbnail(currentItem.FullName);
                               ctx.Post(new SendOrPostCallback((state) => currentItem.Thumbnail = tb),null);
                           }
                       });
               }, new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

    }

    class ListItem : INotifyPropertyChanged
    {
        Tagger.Engine.FileLink _link;
        ImageSource _thumbnail;

        public ListItem(FileLink link)
        {
            _link = link;
        }

        public string Name { get { return _link.Name; } }
        public string FullName { get { return _link.FullName; } }
        public string MD5 { get { return _link.MD5; } }

        public ImageSource Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                OnPropertyChanged("Thumbnail");
            }
        }

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    class BitmapImageThumbnailProvider : IThumbnailProvider
    {

        #region IThumbnailProvider Members

        public ImageSource GetThumbnail(string imagePath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = 200;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(imagePath);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        #endregion
    }

}
