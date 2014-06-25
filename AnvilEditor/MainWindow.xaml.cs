using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json.Converters;
using NLog;
using Xceed.Wpf.Toolkit;

using AnvilEditor.Models;
using System.Reflection;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Create a logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("MainWindow");

        /// <summary>
        /// A command for entering edit mode
        /// </summary>
        public static RoutedCommand EnterEditModeCommand = new RoutedCommand();

        /// <summary>
        /// A command for entering create mode
        /// </summary>
        public static RoutedCommand EnterCreateModeCommand = new RoutedCommand();

        /// <summary>
        /// A command for entering zoom mode
        /// </summary>
        public static RoutedCommand EnterZoomModeCommand = new RoutedCommand();

        /// <summary>
        /// A command for entering respawn placement mode
        /// </summary>
        public static RoutedCommand EnterRespawnModeCommand = new RoutedCommand();

        /// <summary>
        /// A command for entering ambient placement mode
        /// </summary>
        public static RoutedCommand EnterAmbientModeCommand = new RoutedCommand();

        /// <summary>
        /// A command for showing the SQM parser / editor
        /// </summary>
        public static RoutedCommand ShowSQMEditorCommand = new RoutedCommand();

        /// <summary>
        /// A command which refreshes the in-memory mission from the loaded SQM file
        /// </summary>
        public static RoutedCommand RefreshMissionFromSqmCommand = new RoutedCommand();

        /// <summary>
        /// A command which checks for framework updates and provides a download
        /// </summary>
        public static RoutedCommand CheckForUpdatesCommand = new RoutedCommand();

        /// <summary>
        /// A command which checks for framework updates and provides a download
        /// </summary>
        public static RoutedCommand ManualFrameworkUpdateCommand = new RoutedCommand();

        /// A command which causes a completely new "clean" build to be make, deleting all old files
        /// </summary>
        public static RoutedCommand PerformCleanBuildCommand = new RoutedCommand();

        /// <summary>
        /// The unscaled X size of the map image control
        /// </summary>
        public static double ScreenXMax = 600;

        /// <summary>
        /// The unscaled Y size of the map image control
        /// </summary>
        public static double ScreenYMax = 600;

        /// <summary>
        /// The minimum X map coordinate for the given map image
        /// </summary>
        public static int MapXMin;

        /// <summary>
        /// The maximum X map coordinate for the given map image
        /// </summary>
        public static int MapXMax;

        /// <summary>
        /// The minimum Y map coordinate for the given map image
        /// </summary>
        public static int MapYMin;

        /// <summary>
        /// The maximum Y map coordinate for the given map image
        /// </summary>
        public static int MapYMax;

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
        /// A flag to indicate when the application is first opened
        /// </summary>
        private bool IsLoading = true;

        /// <summary>
        /// The currently selected objective
        /// </summary>
        private ObjectiveBase selectedObjective;

        /// <summary>
        /// The shapes used to display the objective
        /// </summary>
        private readonly List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// Holds the last error from the mission sense checking
        /// </summary>
        private string lintError = string.Empty;

        /// <summary>
        /// The type of object being placed in create mode
        /// </summary>
        private ObjectPlacementTypes placementType = ObjectPlacementTypes.Objective;

        /// <summary>
        /// Loads and displays the main window
        /// </summary>
        public MainWindow()
        {
         
            /// get the version number
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            Log.Debug("-------------------------------------------------");
            Log.Debug("Launching Anvil Editor");
            Log.Debug("Application Version: {0}", version);

            InitializeComponent();            
            this.Title = string.Format("Anvil Editor v{0} (Framework v{1})",
                version,
                AnvilEditor.Properties.Settings.Default.FrameworkVersion
            );

            // update the UI
            this.NewButtonClick(new object(), new RoutedEventArgs());
            this.ObjectiveProperties.SelectedObject = this.mission;
            this.IsLoading = false;

            // check for null settings 
            if (AnvilEditor.Properties.Settings.Default.RecentItems == null)
            {
                Log.Debug("Initialised recent items list");
                AnvilEditor.Properties.Settings.Default.RecentItems = new System.Collections.Specialized.StringCollection();
                AnvilEditor.Properties.Settings.Default.Save();
            }

            // update the recent items menu
            this.UpdateRecentMissions();

            // check if this is the first visit
            if (AnvilEditor.Properties.Settings.Default.FirstVisit)
            {
                Log.Debug("Showing first visit prompt");
                var result = System.Windows.MessageBox.Show("It looks like this is the first time you have run Anvil Editor. Would you like to visit the Quick Start guide online?", "Is this your first visit?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start("http://www.anvilproject.com/help/quickstart.html");
                }

                // remove the trigger from future visits
                AnvilEditor.Properties.Settings.Default.FirstVisit = false;
                AnvilEditor.Properties.Settings.Default.Save();
            }

            Log.Debug("Application Loaded");
        }

        /// <summary>
        /// Forces the map to redraw when the user selects another property in the property grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjectivePropertiesSelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemBase> e)
        {
            this.Redraw();
            this.PerformMissionLintChecks();
        }

        /// <summary>
        /// Updates the framework version number from the version.txt file in the mission_raw folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualFrameworkUpdate(object sender, ExecutedRoutedEventArgs e)
        {
            // get a path to the mission_raw folder
            var src = System.IO.Path.Combine(Environment.CurrentDirectory, "mission_raw", "version.txt");

            // read in the version number
            int vers;
            bool worked;
            using (var sr = new StreamReader(src))
            {
                worked = int.TryParse(sr.ReadToEnd(), out vers);
            }
            
            // save to app settings
            if (worked)
            {
                AnvilEditor.Properties.Settings.Default.FrameworkVersion = vers;
                AnvilEditor.Properties.Settings.Default.Save();
            }
            else
            {
                System.Windows.MessageBox.Show("Manual update failed as the version.txt file doesn't appear to hold a valid version number. " + 
                    "You can still create missions using the Anvil Editor, however automatic update downloads may not work as expected.");
            }
        }

        /// <summary>
        /// Runs a lint / sense check on the mission to ensure that it is not CRAZY
        /// and displays the warning button if it is crazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PerformMissionLintChecks()
        {
            this.lintError = OutputGenerator.CompleteChecks(this.mission);

            if (this.lintError == string.Empty)
            {
                this.MissionLintButton.Content = "Mission appears valid";
                this.MissionLintButton.Background = Brushes.White;
                this.MissionLintButton.Foreground = Brushes.Black;
            }
            else
            {
                this.MissionLintButton.Content = "WARNING";
                this.MissionLintButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD63C3C"));
                this.MissionLintButton.Foreground = Brushes.White;

            }
            this.MissionLintButton.Visibility = this.lintError == string.Empty ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Redraws the editor map based on the loaded terrain data stored in the mission
        /// </summary>
        private void UpdateMapFromMission()
        {
            // get the dimensions
            MapXMax = this.mission.MapXMax;
            MapXMin = this.mission.MapXMin;
            MapYMax = this.mission.MapYMax;
            MapYMin = this.mission.MapYMin; 

            // draw the map
            var dataPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data");
            var imagePath = System.IO.Path.Combine(dataPath, "maps", this.mission.ImageName);

            if (!File.Exists(imagePath))
            {
                Log.Warn("Unable to locate the map image - " + imagePath );
                System.Windows.MessageBox.Show("Unable to locate the map image - '" + imagePath + "'. Please check your applications /data/images folder " + 
                    "to ensure the correct map image is present. " + Environment.NewLine + Environment.NewLine + "The default value is 'altis.png', however a custom value " + 
                    "may be specified in your 'mission_data.json` file");
            }
            else
            {
                Log.Debug("Loaded map from " + imagePath);
                var ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                ib.Stretch = Stretch.Uniform;
                this.ObjectiveCanvas.Background = ib;
            }
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
                Log.Debug("Deselected objective");
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
                    Log.Debug("Creating new objective at {0},{1}", pos.X, pos.Y);
                    this.selectedObjective = this.mission.AddObjective(pos);
                    this.ObjectiveProperties.SelectedObject = this.selectedObjective;
                    Log.Debug("  - Objective ID {0} assigned", this.selectedObjective.Id);
                }
                else if (this.placementType == ObjectPlacementTypes.Respawn)
                {
                    this.mission.SetRespawn(pos);
                    this.selectedObjective = null;
                    this.UpdateStatus("Placed respawn at " + this.mission.RespawnX.ToString() + ", " + this.mission.RespawnY.ToString());
                    this.ObjectiveProperties.SelectedObject = this.mission;
                    Log.Debug("Placed respawn at {0},{1}", this.mission.RespawnX, this.mission.RespawnY);
                }
                else if (this.placementType == ObjectPlacementTypes.Ambient)
                {
                    this.ObjectiveProperties.SelectedObject = this.mission.SetAmbientZone(pos);
                    this.UpdateStatus("Placed ambient zone at " + pos.X.ToString() + ", " + pos.Y.ToString());
                    Log.Debug("Placed ambient zone at {0},{1}", pos.X, pos.Y);
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
                return;
            }
            
            else if (this.zooming)
            {
                var oldZoom = this.imageZoom;
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.imageZoom = Math.Min(10, this.imageZoom + 1);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.imageZoom = Math.Max(1, this.imageZoom - 1);
                }

                // get the new center
                if (oldZoom != this.imageZoom)
                {
                    this.imageX = pos.X;
                    this.imageY = pos.Y;
                }
                this.Redraw();

                Log.Info("Zoomed to level {0}", this.imageZoom);
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
                s.Fill = obj == this.selectedObjective ? BrushManager.Selection :
                    (obj.IsOccupied ? BrushManager.Objective : BrushManager.Unoccupied);
                s.Width = 2 * mr;
                s.Height = 2 * mr;
                s.StrokeThickness = obj.NewSpawn ? 1 : 0;
                s.Stroke = BrushManager.NewSpawn;
                s.Tag = obj.Id;
                s.ToolTip = "Objective #" + obj.Id.ToString();
                s.MouseDown += ShapeMouseDown;

                this.ObjectiveCanvas.Children.Add(s);
                Canvas.SetLeft(s, obj.ScreenX - mr);
                Canvas.SetTop(s, obj.ScreenY - mr);
            }

            // draw all the ambient markers
            foreach (var obj in this.mission.AmbientZones)
            {
                var s = new Ellipse();
                s.Fill = obj.IsOccupied ? BrushManager.Ambient : BrushManager.UnoccupiedAmbient;
                s.Width = 2 * mr;
                s.Height = 2 * mr;
                s.StrokeThickness = obj == this.selectedObjective ? 1 : 0;
                s.Stroke = BrushManager.Selection;
                s.Tag = "A_" + obj.Id.ToString();
                s.ToolTip = "Ambient #" + obj.Id.ToString();
                s.MouseDown += ShapeMouseDown;

                this.ObjectiveCanvas.Children.Add(s);
                Canvas.SetLeft(s, obj.ScreenX - mr);
                Canvas.SetTop(s, obj.ScreenY - mr);
            }
            
            // draw the respawn marker
            if (this.mission.RespawnX != 0 || this.mission.RespawnY != 0)
            {
                var rs = new Ellipse();
                rs.Fill = BrushManager.Respawn;
                rs.Width = 2 * mr;
                rs.Height = 2 * mr;
                rs.Tag = "respawn_west";
                rs.ToolTip = "Respawn";
                this.ObjectiveCanvas.Children.Add(rs);
                Canvas.SetLeft(rs, Objective.MapToCanvasX(this.mission.RespawnX) - mr);
                Canvas.SetTop(rs, Objective.MapToCanvasY(this.mission.RespawnY) - mr);
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
            var tagRaw = (Shape)sender;

            if (tagRaw.Tag.ToString().StartsWith("A_"))
            {
				this.selectedObjective = this.mission.AmbientZones[int.Parse(tagRaw.Tag.ToString().Replace("A_",""))];
				this.ObjectiveProperties.SelectedObject = this.selectedObjective;
            }
            else
            {
                var tag = (int)tagRaw.Tag;

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
                    if (tag != this.selectedObjective.Id)
                    {
                        // this is our second item, the first becomes a prereq of the second
                        this.mission.GetObjective(tag).AddPrerequisite(this.selectedObjective.Id);
                        this.UpdateStatus("Set objective #" + this.selectedObjective.Id.ToString() + " as prereq for objective #" + tag.ToString());
                        Log.Debug("Linked objective {0} to {1}", this.selectedObjective.Id, tag);
                    }
                }
            }
               
            this.Redraw();
        }

        /// <summary>
        /// Gets a mission folder into the loadedPath variable
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
        /// Handles UI triggered mission loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadMissionClick(object sender, RoutedEventArgs e)
        {
            this.LoadMission();
        }

        /// <summary>
        /// Loads a mission from file
        /// </summary>
        private void LoadMission(string forcePath = "")
        {
            if (forcePath == "")
            {
                if (!this.GetMissionFolder()) return;
            }
            else
            {
                this.loadedPath = forcePath;
            }

            Log.Debug("Loading mission from {0}", this.loadedPath);
            var missionPath = System.IO.Path.Combine(this.loadedPath, "mission_data.json");

            if (!File.Exists(missionPath)) {
                Log.Warn("  - mission_data.json doesn't exist");
                var res = System.Windows.MessageBox.Show(
                    "This doesn't appear to be a properly formatted Anvil Framework mission. Would you like to create a new one at this location?",
                    "No mission exists", 
                    MessageBoxButton.YesNo
                );

                if (res == MessageBoxResult.No)
                {
                    Log.Debug("  - User aborted mission loading");
                    return;
                }

                Log.Debug("  - User requested a new mission to be created in this folder");

                var path = this.loadedPath;
                this.NewButtonClick(new object(), new RoutedEventArgs());
                this.loadedPath = path;

                this.SaveMission(new object(), new RoutedEventArgs());
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

            this.mission.SQM = FileUtilities.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

            this.UpdateMapFromMission();
            this.PerformMissionLintChecks();
            this.Redraw();
            this.UpdateRecentMissions();
            this.UpdateStatus("Loaded mission");
            Log.Debug("  - Completed mission loading");
        }

        /// <summary>
        /// Saves a mission to file
        /// </summary>
        private void SaveMission(object sender, RoutedEventArgs e)
        {
            Log.Debug("Saving mission");
            if (this.loadedPath == "")
            {
                if (!this.GetMissionFolder())
                {
                    Log.Debug("  - User aborted save");
                    return;
                }
            }

            this.SaveScriptSelection();

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
            
            this.mission.SQM = FileUtilities.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

            this.UpdateRecentMissions();
            this.PerformMissionLintChecks();
            this.UpdateStatus("Saved mission");
            Log.Debug("  - Saved mission");
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
                    Log.Info("Unable to locate an objective with ID {0}", diag.Id);
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
            Log.Debug("Deleting objective ID {0}", this.selectedObjective.Id);

            var t = this.selectedObjective.GetType();
            if (t == typeof(Objective))
            {
                Log.Debug("  - Deleting objective");
                this.mission.DeleteObjective((Objective)this.selectedObjective);
            } 
            else if (t == typeof(AmbientZone))
            {
                Log.Debug("  - Deleting ambient zone");
                this.mission.DeleteAmbientZones(this.selectedObjective as AmbientZone);
            }
            else 
            {
                Log.Warn("  - Ignoring unknown deletion type");
                return;
            }

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
            this.StatusLabel.Text = "[" + (this.EditModeMenuItem.IsChecked ? "EDIT" : (this.CreateModeMenuItem.IsChecked ? "CREATE " + this.placementType.ToString().ToUpper() : "ZOOM")) + "] ";
            this.StatusLabel.Text += status;
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
        /// Performs a completely fresh build of the mission, removing all old files apart from the 'mission_data.json' 
        /// and exporting everything again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerformCleanBuild(object sender, ExecutedRoutedEventArgs e)
        {
            FileUtilities.EmptyMissionDirectory(this.loadedPath);
            this.ExportMissionFiles(sender, e);
        }

        /// <summary>
        /// Generates the mission output into the specified directory. If no directory
        /// is currently stored then it attempts to save the mission.
        /// </summary>
        private void ExportMissionFiles(object sender, RoutedEventArgs e) 
        {
            Log.Debug("Exporting mission");

            // get the output directory
            if (this.loadedPath == "")
            {
                if (!this.GetMissionFolder())
                {
                    Log.Debug("  - No mission path for export, aborting");
                    return;
                }
            }

            this.SaveScriptSelection();
            
            // copy the mission_raw files to the output directory
            var src = System.IO.Path.Combine(Environment.CurrentDirectory, "mission_raw" + System.IO.Path.DirectorySeparatorChar);
            Log.Debug("  - Copying mission files from {0}", src);
            FileUtilities.SafeDirectoryCopy(src, this.loadedPath);

            if (!File.Exists(System.IO.Path.Combine(this.loadedPath, "mission_data.json")))
            {
                Log.Debug("  - Creating mission_data.json file");
                this.SaveMission(new object(), new RoutedEventArgs());
                Log.Debug("  - Done");
            }

            // edit the files
            Log.Debug("  - Creating output generator");
            var generator = new OutputGenerator(this.mission);
            generator.Export(this.loadedPath);

            // read in the mission SQM file
            this.mission.SQM = FileUtilities.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

            this.UpdateStatus("Exported mission to " + this.loadedPath);
            Log.Debug("  - Completed export to {0}", this.loadedPath);
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
            Log.Debug("Creating new map");
            MapData map;

            if (this.IsLoading)
            {
                Log.Debug("  - Loading Altis for first session");
                map = MapDefinitions.Maps["Altis"];
            }
            else
            {
                var nmd = new NewMissionDialog();
                if (nmd.ShowDialog() != true) return;
                map = MapDefinitions.Maps[nmd.SelectedMapName];
                Log.Debug("  - User selected {0}", nmd.SelectedMapName);
            }

            this.selectedObjective = null;
            this.imageX = 0;
            this.imageY = 0;
            this.imageZoom = 2;
            this.loadedPath = "";

            this.mission = new Mission();
            this.mission.MapXMax = map.MapXMax;
            this.mission.MapXMin = map.MapXMin;
            this.mission.MapYMax = map.MapYMax;
            this.mission.MapYMin = map.MapYMin;
            this.mission.ImageName = map.ImageName;

            this.UpdateMapFromMission();
            this.Redraw();
            this.RefreshScripts();
            this.ObjectiveProperties.SelectedObject = this.mission;

            Log.Debug("  - Finished creating new mission");
        }

        /// <summary>
        /// Refreshes the scripts that are available and selected in the script selector box
        /// </summary>
        private void RefreshScripts()
        {
            this.ScriptSelector.Items.Clear();
            foreach (var s in this.mission.AvailableScripts)
            {
                var idx = this.ScriptSelector.Items.Add(s.ToString());

                if (this.mission.IncludedScripts.Contains(s.ToString()))
                {
                    this.ScriptSelector.SelectedItems.Add(s.FriendlyName);
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
        /// Saves the selected scripts to the mission.
        /// 
        /// Should really use a WPF data binding but I'm yet I've never EVER EVER EVER gotten a data binding to actually work.
        /// Maybe one day I too will reach the peak of XAML/C#/WPF coding - getting one of the basic functions to actually work.
        /// Then again probably not.
        /// </summary>
        private void SaveScriptSelection()
        {
            this.mission.IncludedScripts.Clear();
            foreach (string s in this.ScriptSelector.SelectedItems)
            {
                this.mission.IncludedScripts.Add(s);
            }
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
        /// Puts the editor in respawn placement mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterRespawnMode(object sender, RoutedEventArgs e)
        {
            this.CreateModeButtonChecked(sender, e);
            this.placementType = ObjectPlacementTypes.Respawn;
            this.UpdateStatus("Entered respawn placement mode");
        }

        /// <summary>
        /// Puts the editor in ambient zone placement mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterAmbientMode(object sender, RoutedEventArgs e)
        {
            this.CreateModeButtonChecked(sender, e);
            this.placementType = ObjectPlacementTypes.Ambient;
            this.UpdateStatus("Entered ambient placement mode");
        }

        /// <summary>
        /// Show the SQM editor, initially with the current mission's SQM file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSQMEditor(object sender, RoutedEventArgs e)
        {
            Log.Debug("Launching SQM Editor");
            var editor = new SQMParserWindow(this.mission);
            editor.Show();
        }

        /// <summary>
        /// Updates the mission from the mission SQM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshMissionFromSqm(object sender, RoutedEventArgs e)
        {
            // read in the mission SQM file
            Log.Debug("Updating mission from SQM");
            this.mission.SQM = FileUtilities.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

            // read in the changes and display them
            this.mission.UpdateFromSQM();
            this.ObjectiveProperties.Update();
            this.Redraw();
        }

        /// <summary>
        /// Handle mouse wheel zooming over the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObjectiveCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.imageZoom = Math.Min(10, this.imageZoom + 1);
            }
            else if (e.Delta < 0)
            {
                this.imageZoom = Math.Max(1, this.imageZoom - 1);
            }

            var pos = e.GetPosition(this.ObjectiveCanvas);

            this.imageX = pos.X;
            this.imageY = pos.Y;

            this.Redraw();
        }

        /// <summary>
        /// Updates the recent missions menu items
        /// </summary>
        private void UpdateRecentMissions()
        {
            // cycle the list
            if (this.loadedPath != string.Empty)
            {
                // remove the loaded path
                if (AnvilEditor.Properties.Settings.Default.RecentItems.Contains(this.loadedPath))
                {
                    AnvilEditor.Properties.Settings.Default.RecentItems.Remove(this.loadedPath);
                }

                // re add it at the top
                AnvilEditor.Properties.Settings.Default.RecentItems.Insert(
                    AnvilEditor.Properties.Settings.Default.RecentItems.Count, this.loadedPath
                );

                // trim to 5 items
                if (AnvilEditor.Properties.Settings.Default.RecentItems.Count > 5)
                {
                    AnvilEditor.Properties.Settings.Default.RecentItems.RemoveAt(0);
                }

                // save
                AnvilEditor.Properties.Settings.Default.Save();
            }
            
            // add the menu items
            this.RecentItemsMenu.Items.Clear();

            if (AnvilEditor.Properties.Settings.Default.RecentItems.Count > 0)
            {
                this.RecentItemsMenu.IsEnabled = true;
                var removal = new List<string>();
                foreach (var item in AnvilEditor.Properties.Settings.Default.RecentItems)
                {
                    if (!Directory.Exists(item))
                    {
                        removal.Add(item);
                    }
                    else
                    {
                        var menuItem = new MenuItem() { Header = item };
                        menuItem.Click += MenuItemClick;
                        this.RecentItemsMenu.Items.Add(menuItem);
                    }
                }

                // manage missing directories
                foreach (var r in removal)
                {
                    AnvilEditor.Properties.Settings.Default.RecentItems.Remove(r);
                    AnvilEditor.Properties.Settings.Default.Save();
                }
            }
            else
            {
                this.RecentItemsMenu.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles clicking on recent items menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            this.LoadMission(((MenuItem)sender).Header.ToString());
        }

        /// <summary>
        /// Opens the framework updater dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForUpdates(object sender, RoutedEventArgs e)
        {
            var updateWindow = new FrameworkUpdater();
            updateWindow.ShowDialog();

            if (updateWindow.Downloaded)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var version = fvi.FileVersion;

                this.Title = string.Format("Anvil Editor v{0} (Framework v{1})",
                    version,
                    AnvilEditor.Properties.Settings.Default.FrameworkVersion
                );
            }
        }

        /// Shows a message box with mission lint info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MissionLintButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.lintError != string.Empty)
                System.Windows.MessageBox.Show(this.lintError, "The mission contains some potential issues", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// A command that can always be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandAlwaysExecutable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// A command that is only applicable where an objective is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandWithSelectedObjective(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.selectionMode && this.selectedObjective != null;
        }

        /// <summary>
        /// A command that can only be executed if there is a loaded path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandWithLoadedPath(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.loadedPath != "";
        }
    }
}
