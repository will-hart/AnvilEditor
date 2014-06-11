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

using AnvilParser;
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
            var strParser = SQMGrammar.StringObjectParser.Parse(" abc123 = \"7\"; ");
            var intParser = SQMGrammar.IntObjectParser.Parse("   abcd123 = 8; ");
            var fltParser = SQMGrammar.DecimalObjectParser.Parse(" flt =      9.5; ");
            var obj1Parser = SQMGrammar.ObjectParser.Parse(" flt = 9.5; ");
            var obj2Parser = SQMGrammar.ObjectParser.Parse(" int = 5; ");
            var obj3Parser = SQMGrammar.ObjectParser.Parse(" str = \"9.5\"; ");
            var arr1Parser = SQMGrammar.ArrayParser.Parse(" abc123[] = {\"7\", \"2\"}; ");
            var arr2Parser = SQMGrammar.ArrayParser.Parse(" abc123[] = \n{7, \"2\"}; ");
            var arr3Parser = SQMGrammar.ArrayParser.Parse(" abc123[]= {\"7\", \n2}; ");
            var clsParser = SQMGrammar.ClassParser.Parse(" class a1 { a = 1; b = \"2\";};");

            var output = 
                "STR ' abc123 = \"7\"; ' >> " + strParser.Name + " : " + strParser.Value.ToString() + Environment.NewLine +
                "INT '   abcd123 = 8; ' >> " + intParser.Name + " : " + intParser.Value.ToString() + Environment.NewLine +
                "FLT ' flt =      9.5; ' >> " + fltParser.Name + " : " + fltParser.Value.ToString() + Environment.NewLine +
                "OBJ ' flt = 9.5; ' >> " + obj1Parser.Name + " : " + obj1Parser.Value.ToString() + Environment.NewLine +
                "OBJ ' int = 5; ' >> " + obj2Parser.Name + " : " + obj2Parser.Value.ToString() + Environment.NewLine +
                "OBJ ' str = \"9.5\"; ' >> " + obj3Parser.Name + " : " + obj3Parser.Value.ToString() + Environment.NewLine +
                "ARR ' abc123[] = {\"7\", \"2\"}; ' >> " + arr1Parser.Name + " : #" + arr1Parser.Items.Count() + "==>" + string.Join(",", arr1Parser.Items) + Environment.NewLine +
                "ARR ' abc123[] = {7, \"2\"}; ' >> " + arr2Parser.Name + " : #" + arr2Parser.Items.Count() + "==>" + string.Join(",", arr2Parser.Items) + Environment.NewLine +
                "ARR ' abc123[] = {\"7\", 2}; ' >> " + arr3Parser.Name + " : #" + arr3Parser.Items.Count() + "==>" + string.Join(",", arr3Parser.Items) + Environment.NewLine +
                "CLS ' class a1 { a = 1; b= \"2\";};' >> " + clsParser.Name + " : #" + clsParser.Tokens.Count;

            this.SQMOutputBlock.Text = output;
        }

        /// <summary>
        /// Converts the passed SQM text and writes to the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertSQM(object sender, RoutedEventArgs e)
        {
            var parser = SQMGrammar.SQMParser.Parse(this.SQMInputBox.Text);

            // output the re-generated SQM
            this.SQMOutputBlock.Text = parser.ToSQM();

            // build the tree view
            this.SQMTreeView.Items.Clear();
            
            var t = new TreeViewItem();
            t.Header = "MISSION";
            t.Items.Add(this.BuildTree(parser));
            t.IsExpanded = true;

            this.SQMTreeView.Items.Add(t);
        }

        private TreeViewItem BuildTree(ParserClass objects)
        {
            var t = new TreeViewItem();
            t.Header = objects.Name;
            t.IsExpanded = true;

            foreach (var tok in objects.Tokens)
            {
                t.Items.Add(this.BuildTree(tok));
            }

            foreach (var obj in objects.Objects)
            {
                t.Items.Add(this.BuildTree(obj));
            }

            return t;
        }

        private TreeViewItem BuildTree(IParserToken token)
        {
            var t = new TreeViewItem();
            t.Header = token.Name;
            t.Items.Add(token.ToString());
            t.IsExpanded = true;

            return t;
        }
    }
}
