using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var scanner = new Tagger.Engine.FileScanner();
            var path = txtPath.Text;

            Task.Factory.StartNew(() => scanner.ScanFolder(path, "*.jpg", true))
                .ContinueWith((x) => mainView.ItemsSource = x.Result, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var db = new Tagger.Engine.DAL.Database("tagsbase.db3");
            Tagger.Engine.DAL.DatabaseService.RegisterInstance(db);
            var mngr = new Tagger.Engine.StorageManager();

            var scanner = new Tagger.Engine.FileScanner();
            var path = txtPath.Text;
            var fileData = scanner.ScanFolder(path, "*.jpg", true);
            mngr.UpdateStorage(fileData);
        }
    }
}
