using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PhotoTagManager.Settings
{
    class SettingsWndViewModel : Lib.MVVM.ViewModelBase
    {
        private List<SettingsSection> _sections = new List<SettingsSection>();

        public SettingsWndViewModel()
        {
            FillSections();
        }

        private void FillSections()
        {
            
            _sections.Add(SettingsSection.Create("Common", new CommonSettingsViewModel(), new CommonSettingsUI()));

        }

        public IList<SettingsSection> Sections
        {
            get
            {
                return _sections;
            }
        }

        public void Save()
        {
            ApplicationSettings.Default.Save();
        }

    }

    class SettingsSection
    {
        public string Header { get; set; }
        public object Data { get; set; }
        public ContentControl UI { get; set; }

        public static SettingsSection Create(string header, object data, ContentControl ui)
        {
            ui.DataContext = data;

            return new SettingsSection()
            {
                Header = header,
                Data = data,
                UI = ui
            };

        }
    }

    class CommonSettingsViewModel : Lib.MVVM.ViewModelBase
    {
        public CommonSettingsViewModel()
        {

        }

        public string DatabaseLocation 
        {
            get
            {
                return ApplicationSettings.Default.DatabaseLocation;
            }
            set
            {
                ApplicationSettings.Default.DatabaseLocation = value;
                OnPropertyChanged("DatabaseLocation");
            }
        }

    }

}
