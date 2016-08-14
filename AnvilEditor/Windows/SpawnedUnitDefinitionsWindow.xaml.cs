namespace AnvilEditor.Windows
{
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Models;

    [ValueConversion(typeof(List<string>), typeof(string))]
    public class StringListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as List<string>;
            if (val == null)
            {
                return "";
            }

            return string.Join(",", val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            if (strValue != string.Empty)
            {
                return strValue.Split(new char[] { ',' }).ToList();
            }

            return new List<string>();
        }
    }

    /// <summary>
    /// Interaction logic for SpawnedUnitDefinitionsWindow.xaml
    /// </summary>
    public partial class SpawnedUnitDefinitionsWindow : MetroWindow
    {

        /// <summary>
        /// The current EOS configuration
        /// </summary>
        private Dictionary<string, EosSpawnConfiguration> configs;

        /// <summary>
        /// Holds the section keys
        /// </summary>
        private readonly ObservableCollection<string> configKeys;

        /// <summary>
        /// The configuration we are currently editing
        /// </summary>
        public EosSpawnConfiguration SelectedConfig;

        private readonly List<string> RestrictedConfigKeys = new List<string>
        {
            "Default EAST CSAT", 
            "Default WEST NATO",
            "Default IND AAF",
            "Default Civilian",
            "Default WEST FIA"
        };

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="currentConfigs"></param>
        public SpawnedUnitDefinitionsWindow(Dictionary<string, EosSpawnConfiguration> currentConfigs)
        {
            // clone the configs (to allow cancellation of changes)
            this.configs = new Dictionary<string, EosSpawnConfiguration>();
            foreach (var confKey in currentConfigs.Keys)
            {
                this.configs.Add(confKey, currentConfigs[confKey].Clone());
            }

            this.configKeys = new ObservableCollection<string>(this.configs.Keys);

            InitializeComponent();

            this.ConfigSectionKeysComboBox.ItemsSource = this.configKeys;
            this.ConfigSectionKeysComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets the cofniguration set up by the dialog
        /// </summary>
        public Dictionary<string, EosSpawnConfiguration> Config
        {
            get
            {
                return this.configs;
            }
        }

        /// <summary>
        /// Reconfigures the data bindings for the newly selected package
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSectionKeysComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ConfigSectionKeysComboBox.SelectedIndex == -1) return; // wait for selection

            var configKey = this.ConfigSectionKeysComboBox.SelectedValue.ToString();

            if (!this.configs.ContainsKey(configKey)) {
                MessageBox.Show("lolwut?");
                return;
            }

            this.SelectedConfig = this.configs[configKey];
            this.DataContext = this.SelectedConfig;

            this.DeleteButton.IsEnabled = !this.RestrictedConfigKeys.Contains(configKey);
        }

        /// <summary>
        /// Closes the dialog box and discards changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Closes the dialog box and saves changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Adds a new configuration to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddConfigurationButtonClick(object sender, RoutedEventArgs e)
        {
            var title = await this.ShowInputAsync("Specify configuration name", "Enter the name of the EOS spawn configuration:");
            if (title == null || title.Length == 0) return;
            this.AddAndSelectConfig(title, new EosSpawnConfiguration());
        }

        /// <summary>
        /// Duplicates the selected configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DuplicateConfigurationButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedIdx = this.ConfigSectionKeysComboBox.SelectedValue.ToString();
            if (!this.configs.ContainsKey(selectedIdx))
            {
                await this.ShowMessageAsync("No item is selected", "Please select a configuration before clicking duplicate.");
                return;
            }

            var title = await this.ShowInputAsync("Specify configuration name", "Enter the name of the EOS spawn configuration:");
            if (title == null || title.Length == 0) return;


            var conf = this.configs[selectedIdx];
            this.AddAndSelectConfig(title, conf.Clone());
        }

        /// <summary>
        /// Adds a new config item and selects it in the list
        /// </summary>
        /// <param name="title"></param>
        /// <param name="obj"></param>
        private async void AddAndSelectConfig(string title, EosSpawnConfiguration obj)
        {
            if (this.configs.ContainsKey(title))
            {
                await this.ShowMessageAsync("Duplicate configuration names not allowed", "Please select a unique configuration name and try again.");
                return;
            }

            this.configs.Add(title, obj);
            this.configKeys.Add(title);

            var idx = this.ConfigSectionKeysComboBox.Items.IndexOf(title);
            this.ConfigSectionKeysComboBox.SelectedIndex = idx;
        }

        /// <summary>
        /// Deletes the selected config and selects another
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        { 
            var selectedIdx = this.ConfigSectionKeysComboBox.SelectedValue.ToString();
            var idx = this.ConfigSectionKeysComboBox.SelectedIndex;
            if (!this.configs.ContainsKey(selectedIdx))
            {
                await this.ShowMessageAsync("No item is selected", "Please select a configuration before clicking delete.");
                return;
            }

            if (this.RestrictedConfigKeys.Contains(selectedIdx))
            {
                await this.ShowMessageAsync("Cannot delete default confiugrations", "Default configurations cannot be deleted. Please edit these configurations if you would like to modify them.");
                return;
            }

            this.configs.Remove(selectedIdx);
            this.configKeys.Remove(selectedIdx);

            --idx;
            this.ConfigSectionKeysComboBox.SelectedIndex = idx >= 0 ? idx : 0;
        }
    }
}
