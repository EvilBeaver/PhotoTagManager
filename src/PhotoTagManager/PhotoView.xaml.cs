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
        private IThumbnailProvider _thumbnailer;
        private IList<ImageListItem> _imageItems;
        private CancellationTokenSource _cancelThumbnailsUpdate;

        public PhotoView()
        {
            InitializeComponent();
            _thumbnailer = new VistaThumbnailProvider();//new BitmapImageThumbnailProvider();
            
        }


        #region Scaling properties

        public int ImageSizeFactor
        {
            get { return (int)GetValue(ImageSizeFactorProperty); }
            set { SetValue(ImageSizeFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSizeFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSizeFactorProperty =
            DependencyProperty.Register("ImageSizeFactor", typeof(int), typeof(PhotoView), 
            new PropertyMetadata(1, ScalingFactorChangedHandler));

        private static void ScalingFactorChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var photoView = obj as PhotoView;
            if (photoView != null)
            {
                const int X_PROPORTION = 3;
                const int Y_PROPORTION = 4;

                var newX = (double)ScaleXProperty.DefaultMetadata.DefaultValue * (int)args.NewValue;
                var newY = newX * Y_PROPORTION / X_PROPORTION;
                photoView.ScaleX = newX;
                photoView.ScaleY = newY;

                const int RESCAN_FACTOR = 3;
                if ((int)args.OldValue <= RESCAN_FACTOR && (int)args.NewValue > RESCAN_FACTOR)
                {
                    photoView.UpdateThumbnails(ThumbnailQuality.High);
                }
                else if ((int)args.OldValue > RESCAN_FACTOR && (int)args.NewValue <= RESCAN_FACTOR)
                {
                    photoView.UpdateThumbnails(ThumbnailQuality.Normal);
                }
            }
        }

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(PhotoView), new PropertyMetadata(96.0));

        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(PhotoView), new PropertyMetadata(100.0));

        #endregion

        #region Image Sourcing

        public IList<ImageInfo> ImagesSource
        {
            get { return (IList<ImageInfo>)GetValue(ImagesSourceProperty); }
            set { SetValue(ImagesSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagesSourceProperty =
            DependencyProperty.Register("ImagesSource", typeof(IList<ImageInfo>), typeof(PhotoView),
            new PropertyMetadata((dObj, evArgs) =>
                {
                    var control = dObj as PhotoView;
                    if (control != null)
                    {
                        var source = evArgs.NewValue as IList<ImageInfo>;
                        if (source != null)
                        {
                            control.SetItemsSource(source);
                        }
                    }
                })
            );

        private void SetItemsSource(IList<ImageInfo> source)
        {
            var items = source.Select((x) => new ImageListItem(x));
            Task.Factory.StartNew(() =>
                {
                    _imageItems = items.ToList();
                })
                .ContinueWith((task) =>
                    {
                        lvItems.ItemsSource = _imageItems;
                        UpdateThumbnails(ThumbnailQuality.Normal);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateThumbnails(ThumbnailQuality quality)
        {
            var ctx = SynchronizationContext.Current;
            var canceller = new CancellationTokenSource();

            if (_cancelThumbnailsUpdate != null)
            {
                _cancelThumbnailsUpdate.Cancel();
            }

            _cancelThumbnailsUpdate = canceller;

            Task.Factory.StartNew(() =>
            {
                if (_imageItems.Count == 0)
                {
                    return;
                }

                Parallel.ForEach(Partitioner.Create(0, _imageItems.Count), (range, loopState) =>
                {
                    bool isCancelled = false;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var currentItem = _imageItems[i];
                        var tb = _thumbnailer.GetThumbnail(currentItem.FullName, quality);
                        ctx.Post(new SendOrPostCallback((state) => currentItem.Thumbnail = tb), null);
                        if (canceller.IsCancellationRequested)
                        {
                            isCancelled = true;
                            break;
                        }
                    }

                    if (isCancelled)
                    {
                        loopState.Stop();
                    }

                });

                canceller.Dispose();
                if (_cancelThumbnailsUpdate == canceller)
                {
                    _cancelThumbnailsUpdate = null;
                }

            },
            CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

        #endregion

        public bool VerticalScroll
        {
            get { return (bool)GetValue(VerticalScrollProperty); }
            set { SetValue(VerticalScrollProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollProperty =
            DependencyProperty.Register("VerticalScroll", typeof(bool), typeof(PhotoView),
            new PropertyMetadata(
                (dObj, evArgs) =>
                {
                    var control = dObj as PhotoView;
                    bool HasVerticalScroll = (bool)evArgs.NewValue;
                    if (HasVerticalScroll)
                    {
                        ScrollViewer.SetHorizontalScrollBarVisibility(control.lvItems, ScrollBarVisibility.Disabled);
                    }
                    else
                    {
                        ScrollViewer.SetHorizontalScrollBarVisibility(control.lvItems, ScrollBarVisibility.Auto);
                    }
                    
                }));


    }

    class ImageListItem : INotifyPropertyChanged
    {
        Tagger.Engine.FileLink _link;
        ImageSource _thumbnail;

        public ImageListItem(ImageInfo img)
        {
            _link = img.File;
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

}
