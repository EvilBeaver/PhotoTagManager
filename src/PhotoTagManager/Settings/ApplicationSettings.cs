using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PhotoTagManager.Settings
{
    class ApplicationSettings
    {

        private ApplicationSettings()
        {
        }

        private string _dbLocation;

        private void Init()
        {
            UpgradeSettingsIfNeeded();
            ReadSettings();
        }

        private static void UpgradeSettingsIfNeeded()
        {
            string storedValue = Properties.Settings.Default.SettingsVersion;
            string currentValue = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (storedValue != currentValue)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.SettingsVersion = currentValue;
                Properties.Settings.Default.Save();
            }
        }

        private void ReadSettings()
        {
            _dbLocation = Properties.Settings.Default.DatabaseLocation;
        }

        // public properties
        public string DatabaseLocation 
        { 
            get 
            {
                return _dbLocation; 
            }
            set
            {
                _dbLocation = value;
            }
        }


        public void Save()
        {
            Properties.Settings.Default.DatabaseLocation = DatabaseLocation;
            Properties.Settings.Default.Save();
        }
        

        // static
        #region Static

        private static ApplicationSettings _instance = null;

        static ApplicationSettings() { }

        public static ApplicationSettings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApplicationSettings();
                    _instance.Init();
                }

                return _instance;
            }
        }

        #endregion

    }

}
