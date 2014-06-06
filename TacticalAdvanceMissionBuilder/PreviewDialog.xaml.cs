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

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Interaction logic for OutputDialog.xaml
    /// </summary>
    public partial class OutputPreviewDialog : Window
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
            this.InitText.Text = this.generator.Init;
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
