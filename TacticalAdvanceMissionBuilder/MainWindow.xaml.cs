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
using System.Text.RegularExpressions;

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
        /// The path to a file loaded using the "load mission" command or "" if no file loaded
        /// </summary>
        private string loadedPath = "";

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
        private int imageZoom = 2;

        /// <summary>
        /// The point where the last mouse down occurred
        /// </summary>
        private Point lastMouseDownPoint;

        /// <summary>
        /// The currently selected objective
        /// </summary>
        private Objective selectedObjective;

        /// <summary>
        /// The shapes used to display the objective
        /// </summary>
        private readonly List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// The type of object being placed in create mode
        /// </summary>
        private ObjectPlacementTypes placementType = ObjectPlacementTypes.Objective;

        /// <summary>
        /// Loads and displays the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // draw the map
            var ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"arma3_map.1.png", UriKind.Relative));
            this.ObjectiveCanvas.Background = ib;

            // Create the mission
            this.mission = new Mission();
            this.RefreshScripts();
            this.ObjectiveProperties.SelectedObject = this.mission;
            this.Redraw();
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
            this.lastMouseDownPoint = e.GetPosition(this.ObjectiveCanvas);

            if (this.zooming) return;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                // deselect
                this.selectedObjective = null;
                this.ObjectiveProperties.SelectedObject = this.mission;
                this.Redraw();
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.selectionMode) return;
                
                // create
                var pos = e.GetPosition(this.ObjectiveCanvas);
                if (this.placementType == ObjectPlacementTypes.Objective)
                {
                    this.selectedObjective = this.mission.AddObjective(pos);
                    this.ObjectiveProperties.SelectedObject = this.selectedObjective;
                }
                else
                {
                    this.mission.SetRespawn(pos);
                    this.placementType = ObjectPlacementTypes.Objective;
                    this.selectedObjective = null;
                    this.UpdateStatus("Placed respawn at " + this.mission.RespawnX.ToString() + ", " + this.mission.RespawnY.ToString());
                    this.ObjectiveProperties.SelectedObject = this.mission;
                }
            }

            this.Redraw();
        }

        /// <summary>
        /// Handles removing the mouse button on the canvas and checks if we need to pan the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjectiveCanvasMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this.ObjectiveCanvas);
            var dx = this.lastMouseDownPoint.X - pos.X;
            var dy = this.lastMouseDownPoint.Y - pos.Y;

            this.lastMouseDownPoint = new Point(0, 0);

            if (Math.Abs(dx) > 5 || Math.Abs(dy) > 5)
            {
                this.imageX += dx;
                this.imageY += dy;

                this.imageX = Math.Max(0, Math.Min(800, this.imageX));
                this.imageY = Math.Max(0, Math.Min(600, this.imageY));

                this.Redraw();

                this.UpdateStatus(string.Format("Panned to {0}, {1}", this.imageX, this.imageY));
                return;
            }
            
            if (this.zooming)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.imageZoom = Math.Min(10, this.imageZoom + 1);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.imageZoom = Math.Max(1, this.imageZoom - 1);
                }

                // get the new center
                this.imageX = pos.X;
                this.imageY = pos.Y;
                this.Redraw();

                this.UpdateStatus("Set zoom level to " + this.imageZoom.ToString());
            }

            this.lastMouseDownPoint = new Point(0, 0);
        }

        /// <summary>
        /// Redraws the map from scratch
        /// </summary>
        private void Redraw()
        {
            // do the zoom man
            this.MapScale.ScaleX = this.imageZoom;
            this.MapScale.ScaleY = this.imageZoom;
            this.MapScale.CenterX = this.imageX;
            this.MapScale.CenterY = this.imageY;

            var mr = this.MarkerRadius;

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
                    l.StrokeThickness = this.imageZoom < 5 ? 2 : 1;
                    l.StrokeEndLineCap = PenLineCap.Triangle;
                    this.ObjectiveCanvas.Children.Add(l);
                }
            }

            // draw all the objective markers in a second pass to make sure they are on top
            foreach (var obj in this.mission.Objectives)
            {
                var s = new Ellipse();
                s.Fill = obj == this.selectedObjective ? BrushManager.SelectionBrush :
                    (obj.IsOccupied ? BrushManager.ObjectiveBrush : BrushManager.UnoccupiedBrush);
                s.Width = 2 * mr;
                s.Height = 2 * mr;
                s.StrokeThickness = obj.NewSpawn ? 1 : 0;
                s.Stroke = Brushes.Yellow;
                s.Tag = obj.Id;
                s.MouseDown += ShapeMouseDown;

                this.ObjectiveCanvas.Children.Add(s);
                Canvas.SetLeft(s, obj.ScreenX - mr);
                Canvas.SetTop(s, obj.ScreenY - mr);
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
                this.UpdateStatus("Selected objective #" + tag.ToString() + ", hold down shift to start creating links, or press 'Ctrl+X' to delete");
                
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
        /// GEts a mission folder into the loadedPath variable
        /// </summary>
        /// <returns>True if a path was found, false otherwise</returns>
        private bool GetMissionFolder()
        {
            var diag = new System.Windows.Forms.FolderBrowserDialog();
            if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;
            this.loadedPath = diag.SelectedPath;
            return true;
        }

        /// <summary>
        /// Loads a mission from file
        /// </summary>
        private void LoadMission(object sender, RoutedEventArgs e)
        {
            if (!this.GetMissionFolder()) return;

            var missionPath = System.IO.Path.Combine(this.loadedPath, "mission_data.json");

            if (!File.Exists(missionPath)) {
                var res = System.Windows.MessageBox.Show(
                    "This doesn't appear to be a properly formatted Anvil Framework mission. Would you like to create a new one at this location?",
                    "No mission exists", 
                    MessageBoxButton.YesNo
                );

                if (res == MessageBoxResult.No) return;

                var path = this.loadedPath;
                this.NewButtonClick(sender, e);
                this.loadedPath = path;

                this.SaveMission(sender, e);
                return;
            }

            using (var sr = new StreamReader(missionPath))
            {
                var json = sr.ReadToEnd();
                this.mission = JsonConvert.DeserializeObject<Mission>(json);
            }

            this.selectedObjective = null;
            this.ObjectiveProperties.SelectedObject = this.mission;

            this.RefreshScripts();

            this.UpdateStatus("Loaded mission");
            this.Redraw();
        }

        /// <summary>
        /// Saves a mission to file
        /// </summary>
        private void SaveMission(object sender, RoutedEventArgs e)
        {
            if (this.loadedPath == "")
            {
                if (!this.GetMissionFolder()) return;
            }
            
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var sw = new StreamWriter(System.IO.Path.Combine(this.loadedPath, "mission_data.json")))
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
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.DeleteSelectedObjective(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.F1)
            {
                this.EditModeButtonChecked(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.F2)
            {
                this.CreateModeButtonChecked(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.F3)
            {
                this.ZoomModeButtonChecked(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.R)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.EnterRespawnMode(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.S)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.SaveMission(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.O)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.LoadMission(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.E)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.ExportMissionClick(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.F)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.FindObjective(sender, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.N)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.NewButtonClick(sender, new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Finds an objective by ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindObjective(object sender, RoutedEventArgs e)
        {
            var diag = new FindObjectiveDialog();
            diag.ShowDialog();
            if (!diag.Cancelled)
            {
                var obj = this.mission.GetObjective(diag.Id);
                if (obj == null)
                {
                    System.Windows.MessageBox.Show("Unable to locate an objective with ID " + diag.Id.ToString());
                }
                else
                {
                    this.selectedObjective = obj;
                    this.imageX = obj.ScreenX;
                    this.imageY = obj.ScreenY;
                    this.ObjectiveProperties.SelectedObject = obj;
                    this.Redraw();
                }

            }
        }

        /// <summary>
        /// Deletes the selected objective from the mission
        /// </summary>
        private void DeleteSelectedObjective(object sender, RoutedEventArgs e)
        {
            // delete the selected objective
            this.mission.DeleteObjective(this.selectedObjective);
            this.selectedObjective = null;
            this.ObjectiveProperties.SelectedObject = this.mission;
            this.Redraw();
        }

        /// <summary>
        /// Set the status label message
        /// </summary>
        /// <param name="status"></param>
        private void UpdateStatus(string status)
        {
            this.StatusLabel.Content = "[" + (this.EditModeMenuItem.IsChecked ? "EDIT" : (this.CreateModeMenuItem.IsChecked ? "CREATE" : "ZOOM")) + "] ";
            this.StatusLabel.Content += status;
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
        private void PreviewMissionInputs(object sender, RoutedEventArgs e)
        {
            var opd = new OutputPreviewDialog(this.mission);
            opd.ShowDialog();
        }

        /// <summary>
        /// Exports a complete mission to the selected folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportMissionClick(object sender, RoutedEventArgs e)
        {
            this.GenerateMissionFiles();
        }

        /// <summary>
        /// Generates the mission output into the specified directory. If no directory
        /// is currently stored then it attempts to save the mission.
        /// </summary>
        private void GenerateMissionFiles() 
        {
            // get the output directory
            if (this.loadedPath == "")
            {
                if (!this.GetMissionFolder()) return;
            }

            // copy the mission_raw files to the output directory
            var src = System.IO.Path.Combine(Environment.CurrentDirectory, "mission_raw" + System.IO.Path.DirectorySeparatorChar);
            OutputGenerator.SafeDirectoryCopy(src, this.loadedPath);

            // edit the files
            var generator = new OutputGenerator(this.mission);
            generator.Export(this.loadedPath);

            this.UpdateStatus("Exported mission to " + this.loadedPath);
        }

        /// <summary>
        /// Handle toggling the edit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = true;
            this.zooming = false;
            this.placementType = ObjectPlacementTypes.Objective;
                        
            this.ObjectiveCanvas.Cursor = this.selectionMode ? Cursors.Hand : Cursors.Cross;

            this.CreateModeMenuItem.IsChecked = false;
            this.EditModeMenuItem.IsChecked = true;
            this.ZoomModeMenuItem.IsChecked = false;

            this.UpdateStatus("Edit mode set");
        }

        /// <summary>
        /// Handle toggling the create mode button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = false;
            this.zooming = false;
            this.placementType = ObjectPlacementTypes.Objective;

            this.ObjectiveCanvas.Cursor = Cursors.Cross;

            this.CreateModeMenuItem.IsChecked = true;
            this.EditModeMenuItem.IsChecked = false;
            this.ZoomModeMenuItem.IsChecked = false;

            this.UpdateStatus("Create mode set");
        }

        /// <summary>
        /// Handle toggling the zoom mode button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomModeButtonChecked(object sender, RoutedEventArgs e)
        {
            this.selectionMode = true;
            this.zooming = true;
            this.placementType = ObjectPlacementTypes.Objective;
            
            this.ObjectiveCanvas.Cursor = Cursors.UpArrow;

            this.CreateModeMenuItem.IsChecked = false;
            this.EditModeMenuItem.IsChecked = false;
            this.ZoomModeMenuItem.IsChecked = true;

            this.UpdateStatus("Zoom mode set");
        }

        /// <summary>
        /// Creates a new mission, clearing the existing mission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            this.loadedPath = string.Empty;
            this.selectedObjective = null;
            this.mission = this.mission.ClearMission();
            this.imageX = 0;
            this.imageY = 0;
            this.imageZoom = 2;
            this.Redraw();
            this.RefreshScripts();
            this.ObjectiveProperties.SelectedObject = this.mission;
        }

        private void RefreshScripts()
        {
            this.ScriptSelector.Items.Clear();
            foreach (var s in this.mission.AvailableScripts)
            {
                var idx = this.ScriptSelector.Items.Add(s.ToString());

                if (this.mission.IncludedScripts.Contains(s.ToString()))
                {
                    this.ScriptSelector.SelectedItems.Add(s);
                }
            }

            this.ScriptSelector.Focus();
        }

        /// <summary>
        /// Tracks the mouse movement over the canvas and outputs the map coordinates into the status area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjectiveCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this.ObjectiveCanvas);
            var x = Objective.CanvasToMapX(pos.X).ToString();
            var y = Objective.CanvasToMapY(pos.Y).ToString();
            this.UpdateStatus("X: " + x + ", Y: " + y + "   [" + pos.X.ToString() + "," + pos.Y.ToString()+"]");
        }

        /// <summary>
        /// Quits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// Handles the list box selection changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScriptSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var a in e.AddedItems)
            {
                this.mission.UseScript(a.ToString());
            }

            foreach (var r in e.RemovedItems)
            {
                this.mission.RemoveScript(r.ToString());
            }
        }

        /// <summary>
        /// Puts the editor in respawn placement mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterRespawnMode(object sender, RoutedEventArgs e)
        {
            this.CreateModeButtonChecked(sender, e);
            this.placementType = ObjectPlacementTypes.Respawn;
        }

    }
}
