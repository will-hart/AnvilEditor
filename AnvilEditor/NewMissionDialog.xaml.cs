﻿using System;
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
            
            // draw the map
            var dataPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data");
            var missing = false;
            var missingMaps = new List<string>();

            // load up the mission names
            foreach (var map in MapDefinitions.Maps)
            {
                var imagePath = System.IO.Path.Combine(dataPath, "maps", map.Value.ImageName);
                var found = System.IO.File.Exists(imagePath);

                if (found)
                {
                    this.MapListBox.Items.Add(map.Key);
                }
                else
                {
                    missing = true;
                    missingMaps.Add(map.Key);
                }
            }

            if (missing)
            {
                this.MissingMapsLabel.Content += " (" + string.Join(", ", missingMaps) + ")";
                this.MissingMapsLabel.Visibility = Visibility.Visible;
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
        /// Double click a map name to create a new map there immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectButtonClick(sender, new RoutedEventArgs());
        }

        /// <summary>
        /// Gets the map name selected by the dialog
        /// </summary>
        public string SelectedMapName { get; private set; }
    }
}
