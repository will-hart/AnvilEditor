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
    /// Interaction logic for FindObjectiveDialog.xaml
    /// </summary>
    public partial class FindObjectiveDialog : Window
    {
        public FindObjectiveDialog()
        {
            InitializeComponent();
            this.Cancelled = true;
            this.Id = -1;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Cancelled = true;
            this.Close();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            short res;
            if (Int16.TryParse(this.IdTextBox.Text, out res))
            {
                this.Id = res;
                this.Cancelled = false;
                this.Close();
            }
            else
            {
                MessageBox.Show("Unable to understand the ID entered into the search box. Please enter a number");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IdTextBox.Focus();
        }

        public bool Cancelled { get; set; }

        public int Id { get; set; }
    }
}
