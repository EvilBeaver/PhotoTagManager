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
        private IEnumerable<FileLink> _itemsSource;
        
        public PhotoView()
        {
            InitializeComponent();
        }

        public IEnumerable<FileLink> ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                _itemsSource = value;
                var ctx = SynchronizationContext.Current;

                var items = _itemsSource.Select((x) => new ListItem(x, ctx)).ToList();
                lvItems.ItemsSource = items;

                var thumbnails = new BlockingCollection<KeyValuePair<ListItem,ImageSource>>();
                Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Parallel.ForEach(items, (x) =>
                                {
                                    var tb = x.GetThumbnail();
                                    thumbnails.Add(new KeyValuePair<ListItem, ImageSource>(x, tb));
                                });
                        }
                        finally
                        {
                            thumbnails.CompleteAdding();
                        }

                    },new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default);

                Task.Factory.StartNew(() =>
                    {
                        foreach (var item in thumbnails.GetConsumingEnumerable())
                        {
                            ctx.Post(new SendOrPostCallback((state) => item.Key.Thumbnail = item.Value), null);
                        }
                    }, new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default)
                    .ContinueWith((x) => { GC.Collect(); });

                //Task.Factory.StartNew<IList<ListItem>>(() => _itemsSource.AsParallel().Select((x) => new ListItem(x, ctx)).ToList(),
                //    new CancellationToken(),
                //    TaskCreationOptions.None,
                //    TaskScheduler.Default)
                //    .ContinueWith((x) => lvItems.ItemsSource = x.Result, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

    }

    class ListItem : INotifyPropertyChanged
    {
        Tagger.Engine.FileLink _link;
        ImageSource _thumbnail;
        SynchronizationContext _ctx;

        public ListItem(FileLink link, SynchronizationContext context)
        {
            _link = link;
            _ctx = context;
            //RefreshThumbnail();
        }

        public ImageSource GetThumbnail()
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = 200;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(_link.FullName);
            bi.EndInit();
            bi.Freeze();
            return bi;
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


        public void RefreshThumbnail()
        {
            var tb = GetThumbnail();
            _ctx.Post(new SendOrPostCallback((state) => Thumbnail = tb), null);

            //Task.Factory.StartNew(() =>
            //    {
            //        var tb = GetThumbnail();
            //        _ctx.Post(new SendOrPostCallback((state) => Thumbnail = tb), null);
            //    });
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

}
