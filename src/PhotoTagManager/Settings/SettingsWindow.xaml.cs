using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoTagManager.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsWndViewModel _model;
        
        public SettingsWindow()
        {
            InitializeComponent();

            _model = new SettingsWndViewModel();
            this.DataContext = _model;
            SectionsViewer.SelectedIndex = 0;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _model.Save();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
