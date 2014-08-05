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
    /// Interaction logic for IncludedScriptsEditorWindow.xaml
    /// </summary>
    public partial class IncludedScriptsEditorWindow : Window
    {
        private readonly ScriptInclude script = new ScriptInclude();

        public IncludedScriptsEditorWindow()
        {
            this.DataContext = this.script;
            InitializeComponent();
        }
        
        private void CanceButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public ScriptInclude Script 
        { 
            get 
            {
                return this.script; 
            }
        }
    }
}
