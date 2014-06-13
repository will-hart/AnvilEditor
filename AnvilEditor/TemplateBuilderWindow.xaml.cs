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
using AnvilEditor.Templates;

using AnvilParser;
using AnvilParser.Grammar;
using AnvilParser.Tokens;

using Newtonsoft.Json;
using Sprache;
using System.IO;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TemplateBuilderWindow : Window
    {
        /// <summary>
        /// A list of template components
        /// </summary>
        private readonly InjectedTemplate template = new InjectedTemplate();

        /// <summary>
        /// The currently selected component
        /// </summary>
        private ParserClass selectedComponent;

        /// <summary>
        /// The currently selected key in the UI
        /// </summary>
        private string selectedKey;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="missionModel">The mission we are currently editing</param>
        public TemplateBuilderWindow()
        {
            InitializeComponent();

            // set the data contexts because this never seems to work in XAML
            this.DataContext = this.template;
        }

        /// <summary>
        /// Closes the dialogue window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Adds a new component to the template and selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddComponentClick(object sender, RoutedEventArgs e)
        {
            var po = new ParserClass("Component" + this.template.Components.Keys.Count.ToString());
            this.template.Components.Add(po.Name, po);
            this.ComponentListBox.Items.Add(po.Name);
        }
        
        /// <summary>
        /// Builds a tree from the given parser object
        /// </summary>
        /// <param name="parser"></param>
        protected void BuildTree(ParserClass parser, TreeView tree)
        {
            // build the tree view
            tree.Items.Clear();

            var t = new TreeViewItem();
            tree.Items.Add(this.BuildTreeNodes(parser));
        }

        /// <summary>
        /// Recursively constructs the current node
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        protected TreeViewItem BuildTreeNodes(ParserClass objects)
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

        /// <summary>
        /// Recursively constructs the current node
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected TreeViewItem BuildTreeNodes(IParserToken token)
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

        /// <summary>
        /// Saves the template to the templates folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTemplateClick(object sender, RoutedEventArgs e)
        {
            var dataPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data");
            var templatePath = System.IO.Path.Combine(dataPath, "templates");
            var diag = new Microsoft.Win32.SaveFileDialog();
            diag.InitialDirectory = templatePath;

            if (diag.ShowDialog() != true) return;

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var sw = new StreamWriter(diag.FileName))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this.template);
                }
            }
        }

        /// <summary>
        /// Changes the item being edited in the template editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComponentListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            this.selectedKey = e.AddedItems[0].ToString();
            this.selectedComponent = (ParserClass)this.template.Components[this.selectedKey];
            this.SQMInputBox.Text = this.selectedComponent.ToSQM();
            this.BuildTree(this.selectedComponent, this.SQMTreeView);
            this.ComponentPathBox.Text = this.selectedKey;
        }

        /// <summary>
        /// Called when the SQM input box loses focus, so the SQM objects can be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SqmInputLostFocus(object sender, RoutedEventArgs e)
        {
            var sqm = SQMGrammar.SQMParser.Parse(this.SQMInputBox.Text);
            this.template.Components[this.selectedKey] = sqm;
            this.BuildTree(sqm, this.SQMTreeView);
        }

        /// <summary>
        /// When the component path text box loses focus, update the keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComponentPathLostFocus(object sender, RoutedEventArgs e)
        {
            var newKey = this.ComponentPathBox.Text;
            if (newKey == this.selectedKey) return;

            // update the template
            this.template.Components.Remove(this.selectedKey);
            this.template.Components.Add(newKey, this.selectedComponent);

            // update the list box
            var idx = this.ComponentListBox.Items.IndexOf(this.selectedKey);
            this.ComponentListBox.Items.RemoveAt(idx);
            this.ComponentListBox.Items.Insert(idx, newKey);
            this.ComponentListBox.SelectedIndex = idx;

            this.selectedKey = newKey;
        }
    }
}
