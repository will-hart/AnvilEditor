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
    /// Interaction logic for BriefingWindow.xaml
    /// </summary>
    public partial class BriefingWindow : Window
    {
        private Mission mission;

        public BriefingWindow(Mission mission)
        {
            this.mission = mission;
            this.DataContext = this.mission;
            InitializeComponent();
        }

        private void AddBriefingItemButtonClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
