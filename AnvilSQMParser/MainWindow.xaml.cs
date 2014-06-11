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

            this.TestObjectToSQMClick(new object(), new RoutedEventArgs());
        }

        private void TestObjectToSQMClick(object sender, RoutedEventArgs e)
        {
            var mission = new MissionBase("root", true);

            this.BuildTree(mission);

            this.SQMInputBox.Text = mission.ToSQM();

        }

        /// <summary>
        /// Converts the passed SQM text and writes to the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertSQM(object sender, RoutedEventArgs e)
        {
            var parser = SQMGrammar.SQMParser.Parse(this.SQMInputBox.Text);
            
            // inject a test item
            parser.Inject("Mission.version", 99);
            parser.Inject("Mission.Intel.briefingName", "Injected by TG Will");

            this.BuildTree(parser);
        }

        private void BuildTree(ParserClass parser)
        {
            // build the tree view
            this.SQMTreeView.Items.Clear();

            var t = new TreeViewItem();
            this.SQMTreeView.Items.Add(this.BuildTreeNodes(parser));
        }

        private TreeViewItem BuildTreeNodes(ParserClass objects)
        {
            var t = new TreeViewItem();
            t.Header = objects.Name;
            t.IsExpanded = true;

            foreach (var tok in objects.Tokens)
            {
                t.Items.Add(this.BuildTreeNodes(tok));
            }

            foreach (var obj in objects.Objects)
            {
                t.Items.Add(this.BuildTreeNodes(obj));
            }

            return t;
        }

        private TreeViewItem BuildTreeNodes(IParserToken token)
        {
            var t = new TreeViewItem();

            if (token.GetType() == typeof(ParserArray)) 
            {
                t.Header = token.Name + "[" + ((ParserArray)token).Items.Count.ToString() + "]";
                foreach (var i in ((ParserArray)token).Items)
                {
                    var t2 = new TreeViewItem();
                    t2.Header = i.ToString();
                    t.Items.Add(t2);
                }
            } 
            else 
            {
                t.Header = token.Name + " = " + token.ToString();
            }
            return t;
        }
    }
}
