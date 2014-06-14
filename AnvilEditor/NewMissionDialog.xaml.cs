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
using System.Windows.Shapes;

using AnvilEditor.Models;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for NewMissionDialog.xaml
    /// </summary>
    public partial class NewMissionDialog : Window
    {
        public NewMissionDialog()
        {
            InitializeComponent();

            // load up the mission names
            foreach (var map in MapDefinitions.MapNames)
            {
                this.MapListBox.Items.Add(map);
            }

            // select Altis
            this.MapListBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Update the credits box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedMapName = this.MapListBox.SelectedValue.ToString();
            this.MapDetailsTextBox.Text = MapDefinitions.Maps[this.SelectedMapName].ToString();
        }

        /// <summary>
        /// Close the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Gets the map name selected by the dialog
        /// </summary>
        public string SelectedMapName { get; private set; }

    }
}
