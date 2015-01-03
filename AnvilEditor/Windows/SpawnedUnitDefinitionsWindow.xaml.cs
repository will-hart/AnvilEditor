namespace AnvilEditor.Windows
{
    using MahApps.Metro.Controls;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using AnvilEditor.Models;

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

            return DependencyProperty.UnsetValue;
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
            var configKey = this.ConfigSectionKeysComboBox.SelectedValue.ToString();

            if (!this.configs.ContainsKey(configKey)) {
                MessageBox.Show("lolwut?");
                return;
            }

            this.SelectedConfig = this.configs[configKey];
            this.DataContext = this.SelectedConfig;
        }
    }
}
