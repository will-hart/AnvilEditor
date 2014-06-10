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
using System.Windows.Navigation;
using System.Windows.Shapes;

using AnvilParser.Grammar;
using AnvilParser.Tokens;
using Sprache;

namespace AnvilSQMParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var strParser = SQMGrammar.StringObjectParser.Parse(" str = \"7\"; ");
            var intParser = SQMGrammar.IntObjectParser.Parse("   int1 = 8; ");
            var fltParser = SQMGrammar.FloatObjectParser.Parse(" flt =      9.5; ");
            //var arrParser = SQMGrammar.ArrayParser.Parse(" arr[] = {\"7\", 2}; "); 

            MessageBox.Show(
                "STR ' abc123 = \"7\"; ' >> " + strParser.Name + " : " + strParser.Value.ToString() + Environment.NewLine +
                "INT '   abcd123 = 8; ' >> " + intParser.Name + " : " + intParser.Value.ToString() + Environment.NewLine +
                "INT ' flt =      9.5; ' >> " + fltParser.Name + " : " + fltParser.Value.ToString() + Environment.NewLine //+
               // "ARR ' abc123[] = {\"7\", 2}; ' >> " + arrParser.Name + " : #" + arrParser.Items.Count() + Environment.NewLine
            );
        }
    }
}
