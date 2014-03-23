using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using PhotoTagManager.Settings;

namespace PhotoTagManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (HasValidSettings())
            {
                RunMainWindow();
            }
            else
            {

                try
                {
                    SaveDefaultDBPath();
                }
                catch
                {
                    GetInitialSettings();
                }
            }
        }

        private static void SaveDefaultDBPath()
        {
            var usrPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbDefaultPath = System.IO.Path.Combine(usrPath, "PhotoTagManager");
            System.IO.Directory.CreateDirectory(dbDefaultPath);
            ApplicationSettings.Default.DatabaseLocation = dbDefaultPath;
            ApplicationSettings.Default.Save();
        }

        private void GetInitialSettings()
        {
            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            var wnd = new SettingsWindow();
            wnd.Closed += wnd_Closed;
            wnd.Show();
        }

        void wnd_Closed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= wnd_Closed;
            if (!HasValidSettings())
            {
                Shutdown();
            }
            else
            {
                RunMainWindow();
            }
        }

        private void RunMainWindow()
        {

            ConnectDatabase();

            var wnd = new MainWindow();
            this.MainWindow = wnd;
            ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            wnd.Show();
        }

        private void ConnectDatabase()
        {
            var path = System.IO.Path.Combine(ApplicationSettings.Default.DatabaseLocation, DatabaseFile());
            var db = new Tagger.Engine.DAL.Database(path);
            Tagger.Engine.DAL.DatabaseService.RegisterInstance(db);
        }

        private bool HasValidSettings()
        {
            var dbPath = ApplicationSettings.Default.DatabaseLocation;
            if (String.IsNullOrWhiteSpace(dbPath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string DatabaseFile()
        {
            return "tagsbase.db3";
        }

    }
}
