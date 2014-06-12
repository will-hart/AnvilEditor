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

using AnvilEditor.Models;

using AnvilParser;
using AnvilParser.Grammar;
using AnvilParser.Tokens;

using Newtonsoft.Json;
using Sprache;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SQMParserWindow : Window
    {
        /// <summary>
        /// The mission that was parsed from the input panel
        /// </summary>
        private MissionBase mission;

        /// <summary>
        /// A reference to the mission we are currently editing so we can view the SQM tree
        /// </summary>
        private Mission missionModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="missionModel">The mission we are currently editing</param>
        public SQMParserWindow(Mission missionModel)
        {
            InitializeComponent();
            this.missionModel = missionModel;
            this.TestObjectToSQMClick(new object(), new RoutedEventArgs());
        }

        /// <summary>
        /// Populate the tree with the base mission we are currently editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestObjectToSQMClick(object sender, RoutedEventArgs e)
        {
            this.BuildTree(this.missionModel.SQM);
            this.SQMInputBox.Text = this.missionModel.SQM.ToSQM();
        }

        /// <summary>
        /// Converts the passed SQM text and writes to the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertSQM(object sender, RoutedEventArgs e)
        {
            this.mission = SQMGrammar.SQMParser.Parse(this.SQMInputBox.Text);
            this.BuildTree(this.mission);
        }

        /// <summary>
        /// Exports the current SQM Tree to JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToJsonClick(object sender, RoutedEventArgs e)
        {
            // build the JSON
            var output = string.Empty;
            Clipboard.SetText(JsonConvert.SerializeObject(
                this.mission, 
                new JsonSerializerSettings() { 
                    Formatting=Formatting.Indented, NullValueHandling = NullValueHandling.Ignore 
                }
            ));
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
            if (objects == null) return new TreeViewItem();

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
