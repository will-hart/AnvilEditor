﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using AnvilEditor.Models;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for OutputDialog.xaml
    /// </summary>
    public partial class OutputPreviewDialog
    {
        /// <summary>
        /// Holds the mission which all data is generated from
        /// </summary>
        private readonly OutputGenerator generator;

        public OutputPreviewDialog(Mission mission)
        {
            InitializeComponent();

            this.generator = new OutputGenerator(mission);
            this.MarkerText.Text = this.generator.Markers;
            this.InitText.Text = this.generator.ObjectiveList;
        }

        /// <summary>
        /// Closes the dialog when copying is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Selects all the text in a text box when the user clicks it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxClick(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.SelectAll();
        }
    }
}
