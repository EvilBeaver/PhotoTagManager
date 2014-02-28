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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoTagManager.Settings
{
    /// <summary>
    /// Interaction logic for CommonSettingsUI.xaml
    /// </summary>
    public partial class CommonSettingsUI : UserControl
    {
        public CommonSettingsUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Avalon.Windows.Dialogs.FolderBrowserDialog();
            dlg.BrowseFiles = false;
            if (dlg.ShowDialog() == true)
            {
                txtDatabaseLocation.Text = dlg.SelectedPath;
                txtDatabaseLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
