using System;
using System.Collections.Generic;
using System.IO;
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

using Newtonsoft.Json;
using Microsoft.Win32;
using Newtonsoft.Json.Converters;

using Xceed.Wpf.Toolkit;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The mission being edited
        /// </summary>
        private Mission mission;

        /// <summary>
        /// A brush for drawing in objective ellipses
        /// </summary>
        private readonly SolidColorBrush objectiveBrush = new SolidColorBrush();

        /// <summary>
        /// A brush for highlighting the selected objective
        /// </summary>
        private readonly SolidColorBrush selectionBrush = new SolidColorBrush();

        /// <summary>
        /// Flag set to true when we have the z key held down and are zooming
        /// </summary>
        private bool zooming = false;

        /// <summary>
        /// Flag set to true when we have the shift key held down and are linking objectives
        /// </summary>
        private bool linking = false;

        /// <summary>
        /// True when we should select rather than create
        /// </summary>
        private bool selectionMode = true;
        
        /// <summary>
        /// The X position of the top left corner of the image display
        /// </summary>
        private double imageX = 0;

        /// <summary>
        /// The Y position of the top left corner of the image display
        /// </summary>
        private double imageY = 0;

        /// <summary>
        /// The zoom level of the image, defaults to 3
        /// </summary>
        private int imageZoom = 1;

        /// <summary>
        /// The currently selected objective
        /// </summary>
        private Objective selectedObjective;

        /// <summary>
        /// The shapes used to display the objective
        /// </summary>
        private readonly List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// Loads and displays the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.objectiveBrush = new SolidColorBrush();
            this.objectiveBrush.Color = Color.FromArgb(255, 0, 0, 0);
            this.selectionBrush.Color = Color.FromArgb(255, 255, 0, 0);

            var ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"arma3_map.1.png", UriKind.Relative));
            this.ObjectiveCanvas.Background = ib;

            this.mission = new Mission();
            this.Redraw();

            this.EditModeButton.IsChecked = true;
        }

        /// <summary>
        /// Handles clicking to create new objective and dragging to move the map
        /// Shift clicking zooms in and ctrl click zooms out
        /// Right click deselects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.zooming)
            {
                if (e.LeftButton == MouseButtonState.Pressed) 
                {
                    this.imageZoom = Math.Min(10, this.imageZoom + 1);
                } 
                else if (e.RightButton == MouseButtonState.Pressed) 
                {   
                    this.imageZoom = Math.Max(1, this.imageZoom - 1);
                }

                // get the new center
                var pos = e.GetPosition(this.ObjectiveCanvas);
                this.imageX = pos.X;
                this.imageY = pos.Y;

                // do the zoom man
                this.MapScale.ScaleX = this.imageZoom;
                this.MapScale.ScaleY = this.imageZoom;
                this.MapScale.CenterX = this.imageX;
                this.MapScale.CenterY = this.imageY;

                this.UpdateStatus("Set zoom level to " + this.imageZoom.ToString());
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                // deselect
                selectedObjective = null;
                this.Redraw();
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.selectionMode)
                {
                    // create
                    var pos = e.GetPosition(this.ObjectiveCanvas);
                    this.selectedObjective = this.mission.AddObjective(pos);

                    // bind the property grid
                    this.ObjectiveProperties.SelectedObject = this.selectedObjective;
                }
            }

            this.Redraw();
        }

        /// <summary>
        /// Redraws the map from scratch
        /// </summary>
        private void Redraw()
        {
            // remove all the old shapes from the grid
            this.ObjectiveCanvas.Children.Clear();

            // draw all the pre-requisites on
            foreach (var obj in this.mission.Objectives)
            {
                foreach (var p in obj.Prerequisites)
                {
                    var pre = this.mission.GetObjective(p);
                    var l = new Line();
                    l.X1 = pre.ScreenX;
                    l.Y1 = pre.ScreenY;
                    l.X2 = obj.ScreenX;
                    l.Y2 = obj.ScreenY;
                    l.Stroke = obj == this.selectedObjective ? Brushes.Green : Brushes.Black;
                    l.StrokeThickness = 2;
                    l.StrokeEndLineCap = PenLineCap.Triangle;
                    this.ObjectiveCanvas.Children.Add(l);
                }
            }

            // draw all the objective markers in a second pass to make sure they are on top
            foreach (var obj in this.mission.Objectives)
            {
                var s = new Ellipse();
                s.Fill = obj == this.selectedObjective ? this.selectionBrush : this.objectiveBrush;
                s.Width = 2 * this.MarkerRadius;
                s.Height = 2 * this.MarkerRadius;
                s.StrokeThickness = 0;
                s.Tag = obj.Id;
                s.MouseDown += ShapeMouseDown;

                this.ObjectiveCanvas.Children.Add(s);
                Canvas.SetLeft(s, obj.ScreenX - this.MarkerRadius);
                Canvas.SetTop(s, obj.ScreenY - this.MarkerRadius);
            }
        }

        /// <summary>
        /// Handles clicking on a shape in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShapeMouseDown(object sender, MouseButtonEventArgs e)
        {
            // get the id of the selected item
            var tag = (int)((Shape)sender).Tag;

            if (this.selectedObjective == null || !this.linking)
            {
                // we have no selection so select the current item
                this.selectedObjective = this.mission.GetObjective(tag);
                this.UpdateStatus("Selected objective #" + tag.ToString() + ", hold down shift to start creating links, or press 'x' to delete");
                
                // bind the property grid
                this.ObjectiveProperties.SelectedObject = this.selectedObjective;
            }
            else if (this.linking)
            {
                // this is our second item, the first becomes a prereq of the second
                this.mission.GetObjective(tag).AddPrerequisite(this.selectedObjective.Id);
                this.UpdateStatus("Set objective #" + this.selectedObjective.Id.ToString() + " as prereq for objective #" + tag.ToString());
            }
               
            this.Redraw();
        }

        /// <summary>
        /// Loads a mission from file
        /// </summary>
        private void LoadMission(object sender, RoutedEventArgs e)
        {
            var diag = new OpenFileDialog();
            if (diag.ShowDialog() != true) return;
            var path = diag.FileName;

            this.ObjectiveCanvas.Children.Clear();

            using (var sr = new StreamReader(path))
            {
                var json = sr.ReadToEnd();
                this.mission = JsonConvert.DeserializeObject<Mission>(json);
            }

            this.UpdateStatus("Loaded mission");
            this.Redraw();
        }

        /// <summary>
        /// Saves a mission to file
        /// </summary>
        private void SaveMission(object sender, RoutedEventArgs e)
        {
            var diag = new SaveFileDialog();
            if (diag.ShowDialog() != true) return;

            var path = diag.FileName;
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var sw = new StreamWriter(path))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this.mission);
                }
            }

            this.UpdateStatus("Saved mission");
        }

        /// <summary>
        /// Handles entering linking mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftShift || e.Key == Key.RightShift) && this.selectionMode)
            {
                this.linking = true;
                this.UpdateStatus("Click another objective to create a link, release shift to stop");
            }
        }

        /// <summary>
        /// Saves key press state on key up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftShift || e.Key == Key.RightShift) && this.selectionMode)
            {
                this.linking = false;
                this.UpdateStatus("");
            } 
            else if (e.Key == Key.X && this.selectionMode && this.selectedObjective != null)
            {
                // delete the selected objective
                this.mission.DeleteObjective(this.selectedObjective);
                this.selectedObjective = null;

                this.Redraw();
            }
        }

        /// <summary>
        /// Set the status label message
        /// </summary>
        /// <param name="status"></param>
        private void UpdateStatus(string status)
        {
            this.StatusLabel.Content = status;
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Returns a marker radius which is dependent on zoom level
        /// </summary>
        internal int MarkerRadius
        {
            get
            {
                return (int)(6 - 0.5 * this.imageZoom);
            }
        }

        /// <summary>
        /// Generates the markers data and framework_init sections and displays them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateMissionInputs(object sender, RoutedEventArgs e)
        {
            var opd = new OutputDialog(this.mission);
            opd.ShowDialog();
        }

        /// <summary>
        /// Exports a complete mission to the selected folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportMissionClick(object sender, RoutedEventArgs e)
        {
            // get the output directory
            var diag = new System.Windows.Forms.FolderBrowserDialog();
            if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            // copy the mission_raw files to the output directory
            var src = System.IO.Path.Combine(Environment.CurrentDirectory, "mission_raw" + System.IO.Path.DirectorySeparatorChar);
            this.DirectoryCopy(src, diag.SelectedPath);

            // edit the files
            var generator = new OutputGenerator(this.mission);
            var fwi = System.IO.Path.Combine(diag.SelectedPath, "framework", "framework_init.sqf");
            var mis = System.IO.Path.Combine(diag.SelectedPath, "mission.sqm");
            this.UpdateFileContents(fwi, "$$$OBJECTIVELIST$$$", generator.Init);
            this.UpdateFileContents(mis, "$$$MARKERS$$$", generator.Markers);

            Xceed.Wpf.Toolkit.MessageBox.Show("Exported mission to: \n\n\t" + diag.SelectedPath);
        }

        /// <summary>
        /// Opens and edits the given file and replaces the MARKER with the text of REPLACEWITH
        /// </summary>
        /// <param name="path">The path of the file to edit</param>
        /// <param name="marker">The marker to replace</param>
        /// <param name="replaceWith">The text to replace the marker with</param>
        private void UpdateFileContents(string path, string marker, string replaceWith)
        {
            var lines = System.IO.File.ReadAllText(path);
            lines = lines.Replace(marker, replaceWith);
            System.IO.File.WriteAllText(path, lines);
        }

        /// <summary>
        /// Copy the raw mission files to the given directory and edit the
        /// framework_init and mission SQM files to add in the generated content
        /// 
        /// Borrowed some code from http://stackoverflow.com/a/12283793/233608
        /// </summary>
        /// <param name="dest">The destination root directory</param>
        private void DirectoryCopy(string src, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

            var dirInfo = new DirectoryInfo(src);
            var files = dirInfo.GetFiles();

            foreach (var tempfile in files)
            {
                tempfile.CopyTo(System.IO.Path.Combine(dest, tempfile.Name), true);
            }

            var dirs = dirInfo.GetDirectories();
            foreach (var tempdir in dirs)
            {
                this.DirectoryCopy(
                    System.IO.Path.Combine(src, tempdir.Name), System.IO.Path.Combine(dest, tempdir.Name));
            }
        }

        private void EditModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = true;
            this.zooming = false;
            this.ZoomModeButton.IsChecked = false;
            this.CreateModeButton.IsChecked = false;
            
            this.ObjectiveCanvas.Cursor = this.selectionMode ? Cursors.Arrow : Cursors.Cross;
            this.UpdateStatus(this.selectionMode ? "Press 's' to enter objective creation mode" : "Press 's' to enter selection mode");
        }

        private void CreateModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = false;
            this.zooming = false;
            this.EditModeButton.IsChecked = false;
            this.ZoomModeButton.IsChecked = false;
            this.ObjectiveCanvas.Cursor = Cursors.Cross;
        }

        private void ZoomModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = true;
            this.zooming = true;
            this.EditModeButton.IsChecked = false;
            this.CreateModeButton.IsChecked = false;
            this.ObjectiveCanvas.Cursor = Cursors.UpArrow;
        }
    }
}
