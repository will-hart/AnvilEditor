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

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    public partial class MainWindow : MetroWindow
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
        /// A command which displays a new briefing editor window
        /// </summary>
        public static RoutedCommand ShowBriefingWindowCommand = new RoutedCommand();

        /// <summary>
        /// A command which displays a new briefing editor window
        /// </summary>
        public static RoutedCommand AddNewSupportedScriptCommand = new RoutedCommand();

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
        /// Are there unsaved changes in the mission?
        /// </summary>
        private bool IsDirty = false;

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
            this.GenerateNewMission("Altis");
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
                this.OfferNewUserHelp();
            }

            Log.Debug("Application Loaded");
        }

        /// <summary>
        /// Shows a message box to the user offering them help on their first visit
        /// </summary>
        private void OfferNewUserHelp()
        {
            Log.Debug("Showing first visit prompt");
            var result = Xceed.Wpf.Toolkit.MessageBox.Show("It looks like this is the first time you have run Anvil Editor. Would you like to visit the Quick Start guide online?",
                "Is this your first visit?", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start("http://www.anvilproject.com/help/quickstart.html");
            }

            // remove the trigger from future visits
            AnvilEditor.Properties.Settings.Default.FirstVisit = false;
            AnvilEditor.Properties.Settings.Default.Save();
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
            this.IsDirty = true;
        }

        /// <summary>
        /// Updates the framework version number from the version.txt file in the mission_raw folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualFrameworkUpdate(object sender, ExecutedRoutedEventArgs e)
        {
            // get a path to the mission_raw folder
            var src = System.IO.Path.Combine(FileUtilities.GetFrameworkSourceFolder, "version.txt");

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
                this.RepopulateVersionTitle();
            }
            else
            {
                this.ShowMessageAsync("Error Updating Framework", "Manual update failed as the version.txt file doesn't appear to hold a valid version number. " + 
                    "You can still create missions using the Anvil Editor, however automatic update downloads may not work as expected.", MessageDialogStyle.Affirmative);
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
        private void UpdateMapFromMission(Mission useMission = null)
        {
            if (useMission == null)
            {
                useMission = this.mission;
            }

            // get the dimensions
            MapXMax = useMission.MapXMax;
            MapXMin = useMission.MapXMin;
            MapYMax = useMission.MapYMax;
            MapYMin = useMission.MapYMin; 

            // draw the map
            var imagePath = System.IO.Path.Combine(FileUtilities.GetDataFolder, "maps", useMission.ImageName);

            if (!File.Exists(imagePath))
            {
                Log.Warn("Unable to locate the map image - " + imagePath );
                var message = "Unable to locate the map image - '" + imagePath + "'. Please check your applications /data/images folder " + 
                    "to ensure the correct map image is present. " + Environment.NewLine + Environment.NewLine + "The default value is 'altis.png', however a custom value " + 
                    "may be specified in your 'mission_data.json` file";

                try
                {
                    this.ShowMessageAsync("Unable to locate map image", message);
                }
                catch (NullReferenceException)
                {
                    System.Windows.MessageBox.Show(message, "Unable to locate teh amp image");
                }
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

                this.IsDirty = true;
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
        /// Generates a rectangle for rendering on the canvas
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="tooltip"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private Rectangle BuildRect(Brush brush, double x, double y, double r, string tooltip = "", object tag = null)
        {
            var s = new Rectangle();
            s.Fill = brush;
            s.Width = 2 * r;
            s.Height = 2 * r;

            if (tooltip != "") 
                s.ToolTip = tooltip;

            if (tag != null)
                s.Tag = tag;
    
            this.ObjectiveCanvas.Children.Add(s);
            Canvas.SetLeft(s, x - r);
            Canvas.SetTop(s, y - r);

            return s;
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
                if (obj.Ammo)
                {
                    var ammo = this.BuildRect(BrushManager.NewAmmo, obj.ScreenX, obj.ScreenY, mr);
                }

                if (obj.Special)
                {
                    var ammo = this.BuildRect(BrushManager.NewSpecial, obj.ScreenX, obj.ScreenY, mr);
                }

                if (obj.NewSpawn)
                {
                    var ammo = this.BuildRect(BrushManager.NewSpawn, obj.ScreenX, obj.ScreenY, mr);
                }

                var s = this.BuildRect(obj.IsOccupied ? BrushManager.Objective : BrushManager.UnoccupiedObjective,
                    obj.ScreenX, obj.ScreenY, mr, "Objective #" + obj.Id.ToString(), obj.Id);
                s.MouseDown += ShapeMouseDown;
            }

            // draw all the ambient markers
            foreach (var obj in this.mission.AmbientZones)
            {
                var a = this.BuildRect(obj.IsOccupied ? BrushManager.Ambient : BrushManager.UnoccupiedAmbient,
                    obj.ScreenX, obj.ScreenY, mr, "Ambient #" + obj.Id.ToString(), "A_" + obj.Id.ToString());
                a.MouseDown += ShapeMouseDown;
            }
            
            // draw the respawn marker
            if (this.mission.RespawnX != 0 || this.mission.RespawnY != 0)
            {
                var rs = this.BuildRect(BrushManager.Respawn, Objective.MapToCanvasX(this.mission.RespawnX),
                    Objective.MapToCanvasY(this.mission.RespawnY), mr, "Respawn", "respawn_west");
            }

            // draw the selected objective
            if (this.selectedObjective != null)
            {
                var rs = this.BuildRect(BrushManager.Selection, this.selectedObjective.ScreenX,
                    this.selectedObjective.ScreenY, mr);
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
                        var obj = this.mission.GetObjective(tag);

                        // Fix link ambient zone
                        if (obj.GetType() != typeof(AmbientZone))
                        {
                            obj.AddPrerequisite(this.selectedObjective.Id);
                            this.UpdateStatus("Set objective #" + this.selectedObjective.Id.ToString() + " as prereq for objective #" + tag.ToString());
                            Log.Debug("Linked objective {0} to {1}", this.selectedObjective.Id, tag);
                        }
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

            // get a useful parent directory
            string topPath;
            if (this.loadedPath.Length == 0)
            {
                if (AnvilEditor.Properties.Settings.Default.RecentItems.Count > 0)
                {
                    topPath = AnvilEditor.Properties.Settings.Default.RecentItems[0];
                }
                else
                {
                    topPath = "";
                }
            }
            else
            {
                topPath = this.loadedPath;
            }

            if (topPath.Length > 0)
            {
                var dir = System.IO.Path.GetDirectoryName(topPath);
                diag.SelectedPath = dir; 
            }

            if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;

            var parts = diag.SelectedPath.Split('.');
            if (parts.Length == 0 || ! MapDefinitions.Maps.ContainsKey(parts.Last())) 
            {
                if (System.Windows.Forms.MessageBox.Show("Your mission folder requires the island name at the end otherwise it won't load in ArmA. Do you want to proceed anyway?", "Folder Name Error", 
                    System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) 
                {
                    return this.GetMissionFolder();
                }
            }
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
        private async void LoadMission(string forcePath = "")
        {
            if (this.IsDirty)
            {
                var result = await this.ShowMessageAsync("There are unsaved changes", "You have unsaved changes in your mission, do you want to save before continuing?",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings() { FirstAuxiliaryButtonText = "Cancel", NegativeButtonText = "No" });
                if (result == MessageDialogResult.FirstAuxiliary) return;
                if (result == MessageDialogResult.Affirmative)
                {
                    this.SaveMission(null, new RoutedEventArgs());
                }
                this.IsDirty = false;
            }

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
                var res = await this.ShowMessageAsync("No Mission Exists",
                    "This doesn't appear to be a properly formatted Anvil Framework mission. Would you like to create a new one at this location?", MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings() { NegativeButtonText = "No" });

                if (res == MessageDialogResult.Negative)
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
                var newMission = JsonConvert.DeserializeObject<Mission>(json);

                if (newMission.ImageName != this.mission.ImageName)
                {
                    // avoid a custom JSON convertor which reads the map before reading in objective X/Y coordinates
                    // by just converting the mission twice. Not ideal but is an edge case so not worth writing 
                    // a whole JSON convertor because of it
                    this.UpdateMapFromMission(newMission);
                    newMission = JsonConvert.DeserializeObject<Mission>(json);
                }

                this.mission = newMission;
            }

            this.selectedObjective = null;
            this.ObjectiveProperties.SelectedObject = this.mission;

            this.RefreshScripts();

            this.mission.SQM = FileUtilities.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

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
            this.IsDirty = false;
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
            this.IsDirty = true;
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
        internal double MarkerRadius
        {
            get
            {
                return 9 - 0.55 * this.imageZoom;
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
        private async void ExportMissionFiles(object sender, RoutedEventArgs e) 
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

            // check we have all the included scripts we require
            var missingScriptFolders = FileUtilities.GetMissingIncludedScriptFolders(this.mission.IncludedScripts, this.mission.AvailableScripts);
            if (missingScriptFolders.Count > 0)
            {
                var result = await this.ShowMessageAsync("There are missing included scripts",
                    "Included scripts are no longer bundled with Anvil. Would you like to attempt to download them manually before continuing? " + Environment.NewLine + Environment.NewLine + 
                    "Note that clicking 'export anyway' will likely mean the mission doesn't work in ArmA. Manual download will open a series of web browser pages where you can download the required folders.",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings() { 
                        AffirmativeButtonText = "Manually Download",
                        NegativeButtonText = "Cancel Export",
                        FirstAuxiliaryButtonText = "Export Anyway"
                    });

                if (result == MessageDialogResult.Negative) return;
                if (result == MessageDialogResult.Affirmative)
                {
                    // open the output folder
                    Process.Start(System.IO.Path.Combine(FileUtilities.GetFrameworkSourceFolder, "fw_scripts"));

                    // open the URL
                    foreach (var script in missingScriptFolders)
                    {
                        if (script.Url != "")
                        {
                            Process.Start(script.Url);
                            await this.ShowMessageAsync("Download " + script.FriendlyName,
                                "Your browser should open up to " + script.Url + ". Download the contents and extract to a folder called" +
                                script.FolderName + " in Anvil's 'fw_scripts' folder. Click OK when done", MessageDialogStyle.Affirmative);
                        }
                    }
                }
            }


            // copy the mission_raw files to the output directory
            var src = FileUtilities.GetFrameworkSourceFolder + System.IO.Path.DirectorySeparatorChar.ToString();
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
            if (!this.NewMissionFlyout.IsOpen)
            {
                // draw the map
                var missing = false;
                var missingMaps = new List<string>();
                this.MapListBox.Items.Clear();

                // load up the mission names
                foreach (var map in MapDefinitions.Maps)
                {
                    var imagePath = System.IO.Path.Combine(FileUtilities.GetDataFolder, "maps", map.Value.ImageName);
                    var found = System.IO.File.Exists(imagePath);

                    if (found)
                    {
                        this.MapListBox.Items.Add(map.Key);
                    }
                    else
                    {
                        missing = true;
                        missingMaps.Add(map.Key);
                    }
                }

                if (missing)
                {
                    this.MissingMapsLabel.Content += " (" + string.Join(", ", missingMaps) + ")";
                    this.MissingMapsLabel.Visibility = Visibility.Visible;
                }

                // select Altis
                this.MapListBox.SelectedIndex = 0;
            }

            this.NewMissionFlyout.IsOpen = !this.NewMissionFlyout.IsOpen;
        }

        /// <summary>
        /// Generates a new mission with the given map name
        /// </summary>
        /// <param name="mapName">The map name to generate the new mission for</param>
        private async void GenerateNewMission(string mapName)
        {
            if (this.IsDirty)
            {
                var result = await this.ShowMessageAsync("There are unsaved changes", "You have unsaved changes in your mission, do you want to save before continuing?",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings() { FirstAuxiliaryButtonText = "Cancel", NegativeButtonText = "No" });
                if (result == MessageDialogResult.FirstAuxiliary) return;
                if (result == MessageDialogResult.Affirmative)
                {
                    this.SaveMission(null, new RoutedEventArgs());
                }
                this.IsDirty = false;
            }

            Log.Debug("Creating new map");
            Log.Debug(" - Loading map {0}", mapName);
            MapData map;
            try
            {
                map = MapDefinitions.Maps[mapName];
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error("Unable to find the map {0}", mapName);
                Log.Error(ex.Message, ex);
                System.Windows.Forms.MessageBox.Show("Unable to create map - unknown map name " + mapName + "!");
                return;
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
            this.IsDirty = false;

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
        private async void ExitApplication(object sender, RoutedEventArgs e)
        {
            if (this.IsDirty)
            {
                var result = await this.ShowMessageAsync("There are unsaved changes", "You have unsaved changes in your mission, do you want to save before continuing?",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings() { FirstAuxiliaryButtonText = "Cancel", NegativeButtonText = "No" });
                if (result == MessageDialogResult.FirstAuxiliary) return;
                if (result == MessageDialogResult.Affirmative)
                {
                    this.SaveMission(null, new RoutedEventArgs());
                }
                this.IsDirty = false;
            }

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
            this.IsDirty = true;
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
                this.imageZoom = Math.Min(15, this.imageZoom + 1);
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
                this.RepopulateVersionTitle();
        }

        /// <summary>
        /// Updates the application version title by reading in the assembly version
        /// </summary>
        private void RepopulateVersionTitle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            this.Title = string.Format("Anvil Editor v{0} (Framework v{1})",
                version,
                AnvilEditor.Properties.Settings.Default.FrameworkVersion
            );
        }

        /// Shows a message box with mission lint info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MissionLintButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.lintError != string.Empty)
                await this.ShowMessageAsync("The mission contains some potential issues", this.lintError, MessageDialogStyle.Affirmative);
        }

        /// <summary>
        /// Shows a briefing editor window for the current mission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBriefingWindow(object sender, ExecutedRoutedEventArgs e)
        {
            var bw = new BriefingWindow(this.mission);
            bw.ShowDialog();
        }

        /// <summary>
        /// Shows the window to add new supported scripts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAddNewSupportedScriptWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.NewIncludeScript == null)
            {
                this.NewIncludeScript = new ScriptInclude();
            }
            this.AddIncludedScriptFlyout.DataContext = this.NewIncludeScript;
            this.AddIncludedScriptFlyout.IsOpen = !this.AddIncludedScriptFlyout.IsOpen;
        }

        /// <summary>
        /// Adds a new included script to the database
        /// </summary>
        private void AddIncludedScript(object sender, RoutedEventArgs e)
        {
            this.mission.AvailableScripts.Add(this.NewIncludeScript);

            // write scripts back to file
            var scriptPath = System.IO.Path.Combine(FileUtilities.GetDataFolder, "supported_scripts.json");
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var sw = new StreamWriter(scriptPath))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this.mission.AvailableScripts);
                }

                this.RefreshScripts();
            }

            this.RefreshScripts();
            this.NewIncludeScript = new ScriptInclude();
            this.AddIncludedScriptFlyout.IsOpen = false;
        }
        
        /// <summary>
        /// Update the credits box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MapListBox.SelectedValue == null)
            {
                this.MapDetailsTextBox.Text = "";
            }
            else
            {
                this.MapDetailsTextBox.Text = MapDefinitions.Maps[this.MapListBox.SelectedValue.ToString()].ToString();
            }
        }

        /// <summary>
        /// Double click a map name to create a new map there immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.NewMissionFlyout.IsOpen = false;
            this.GenerateNewMission(this.MapListBox.SelectedValue.ToString());
        }

        /// <summary>
        /// Handles creating a new map on the map select button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewMapSelectButtonClick(object sender, RoutedEventArgs e)
        {
            this.NewMissionFlyout.IsOpen = false;
            this.GenerateNewMission(this.MapListBox.SelectedValue.ToString());
        }

        /// <summary>
        /// Handles key down in the find objective box. If it is enter, then a find operation will be carried out, if escape then the flyout will be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindIdTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.FindObjectiveFlyout.IsOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                this.PerformFind(this.FindIdTextBox.Text);
            }
        }

        /// <summary>
        /// Shows the find objective by ID flyout and focuses on the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindObjective(object sender, RoutedEventArgs e)
        {
            this.FindObjectiveFlyout.IsOpen = true;
            this.FindIdTextBox.SelectAll();
            this.FindIdTextBox.Focus();
        }

        /// <summary>
        /// Finds an objective by ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PerformFind(string id)
        {
            int objId;
            try
            {
                objId = int.Parse(id);
            }
            catch (FormatException ex)
            {
                Log.Error("Error parsing a find objective id: {0}", id);
                Log.Error(ex.Message, ex);
                return;
            }
            var obj = this.mission.GetObjective(objId);
            if (obj == null)
            {
                Log.Info("Unable to locate an objective with ID {0}", objId);
                await this.ShowMessageAsync("Error finding objective by ID", "Unable to locate an objective with ID " + id);
            }
            else
            {
                this.selectedObjective = obj;
                this.imageX = obj.ScreenX;
                this.imageY = obj.ScreenY;
                this.ObjectiveProperties.SelectedObject = obj;
                this.Redraw();
            }
            this.FindObjectiveFlyout.IsOpen = false;
        }

        /// <summary>
        /// Ensures the application can't be closed in "dirty" state without an alert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.IsDirty)
            {
                var result = Xceed.Wpf.Toolkit.MessageBox.Show("You have unsaved changes in your mission, do you want to save before closing?", "There are unsaved changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    this.SaveMission(null, new RoutedEventArgs());
                }
            }
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

        /// <summary>
        /// An included script being added to the set
        /// </summary>
        private ScriptInclude NewIncludeScript { get; set; }
    }
}
