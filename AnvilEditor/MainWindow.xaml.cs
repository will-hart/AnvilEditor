namespace AnvilEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Newtonsoft.Json;
    using NLog;

    using AnvilEditor.Helpers;
    using AnvilEditor.Models;
    using AnvilEditor.Models.Sources;
    using AnvilEditor.Windows;

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
        /// A command which modifies the default ammobox contents when a new mission is created
        /// </summary>
        public static RoutedCommand ModifyDefaultAmmoboxContentsCommand = new RoutedCommand();

        /// <summary>
        /// A command which modifies the ammobox contents for the loaded mission only
        /// </summary>
        public static RoutedCommand ModifyMissionAmmoboxContentsCommand = new RoutedCommand();

        /// <summary>
        /// A command which modifies the ammobox contents for the loaded mission only
        /// </summary>
        public static RoutedCommand ModifyMissionAmmoboxContents = new RoutedCommand();

        /// <summary>
        /// A command for reverting mission ammo box to the global defaults
        /// </summary>
        public static RoutedCommand RevertMissionAmmoboxToDefaultCommand = new RoutedCommand();

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
        /// The point where the last mouse down occurred
        /// </summary>
        private Point lastMouseDownPoint;

        /// <summary>
        /// Are there unsaved changes in the mission?
        /// </summary>
        private bool IsDirty = false;

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
        /// The default contents for ammoboxes for new missions, initially loaded from JSON files
        /// </summary>
        private List<AmmoboxItem> DefaultAmmoboxContents;

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

            // load defaults
            Log.Debug("Loading Defaults");
            this.LoadDefaults();
            Log.Debug("Defaults loaded");

            // update the UI
            this.GenerateNewMission("Altis");
            this.ObjectiveProperties.SelectedObject = this.mission;

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
        /// Loads in and preoppulates default options
        /// </summary>
        private void LoadDefaults()
        {
            var ammoboxPath = System.IO.Path.Combine(FileHelper.GetDataFolder, "default_ammobox.json");
            
            using (var sw = new StreamReader(ammoboxPath))
            {
                this.DefaultAmmoboxContents = JsonConvert.DeserializeObject<List<AmmoboxItem>>(sw.ReadToEnd());
            }

            // handle the case of an empty JSON file
            if (this.DefaultAmmoboxContents == null)
            {
                this.DefaultAmmoboxContents = new List<AmmoboxItem>();
            }
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
            var src = System.IO.Path.Combine(FileHelper.GetFrameworkSourceFolder, "version.txt");

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
            this.lintError = OutputHelper.CompleteChecks(this.mission);

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
            var imagePath = System.IO.Path.Combine(FileHelper.GetDataFolder, "maps", useMission.ImageName);

            if (!File.Exists(imagePath))
            {
                Log.Warn("Unable to locate the map image - " + imagePath );
                var message = "Unable to locate the map image - '" + imagePath + "'. Please check your applications /data/images folder " + 
                    "to ensure the correct map image is present. " + Environment.NewLine + Environment.NewLine + "The default value is 'altis.png', however a custom value " + 
                    "may be specified in your 'mission_data.json` file";

                // attempt to show error using MahApps, but if it hasn't been instantiated yet revert to system message box
                try
                {
                    this.ShowMessageAsync("Unable to locate map image", message);
                }
                catch (NullReferenceException)
                {
                    System.Windows.MessageBox.Show(message, "Unable to locate the map image");
                }
            }
            else
            {
                // dispose of the old image and force collection - possibly help #26?
                this.ObjectiveCanvas.Background = null;
                GC.Collect();

                // load in the new image
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
                RenderHelper.SelectedObjective = null;
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
                    RenderHelper.SelectedObjective = this.mission.AddObjective(pos);
                    this.ObjectiveProperties.SelectedObject = RenderHelper.SelectedObjective;
                    Log.Debug("  - Objective ID {0} assigned", RenderHelper.SelectedObjective.Id);
                }
                else if (this.placementType == ObjectPlacementTypes.Respawn)
                {
                    this.mission.SetRespawn(pos);
                    RenderHelper.SelectedObjective = null;
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
                RenderHelper.ImageX += dx;
                RenderHelper.ImageY += dy;

                RenderHelper.ImageX = Math.Max(0, Math.Min(800, RenderHelper.ImageX));
                RenderHelper.ImageY = Math.Max(0, Math.Min(600, RenderHelper.ImageY));

                this.Redraw();
                return;
            }
            
            else if (this.zooming)
            {
                RenderHelper.ZoomImage(e.ChangedButton == MouseButton.Left, e.GetPosition(this), pos);
                this.Redraw();
            }

            this.lastMouseDownPoint = new Point(0, 0);
        }
        
        /// <summary>
        /// Redraws the map from scratch
        /// </summary>
        private void Redraw()
        {
            // set the canvas transformation
            this.MapScale.ScaleX = RenderHelper.ImageZoom;
            this.MapScale.ScaleY = RenderHelper.ImageZoom;
            this.MapScale.CenterX = RenderHelper.ImageX;
            this.MapScale.CenterY = RenderHelper.ImageY;

            // render the mission
            RenderHelper.Render(this.ObjectiveCanvas, this.mission, RenderHelper.SelectedObjective, this.ShapeMouseDown);
        }

        /// <summary>
        /// Handles clicking on a shape in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapeMouseDown(object sender, MouseButtonEventArgs e)
        {
            // get the id of the selected item
            var tagRaw = (Shape)sender;

            if (tagRaw.Tag.ToString().StartsWith("A_"))
            {
				RenderHelper.SelectedObjective = this.mission.AmbientZones[int.Parse(tagRaw.Tag.ToString().Replace("A_",""))];
				this.ObjectiveProperties.SelectedObject = RenderHelper.SelectedObjective;
            }
            else
            {
                var tag = (int)tagRaw.Tag;

                if (RenderHelper.SelectedObjective == null || !this.linking)
                {
                    // we have no selection so select the current item
                    RenderHelper.SelectedObjective = this.mission.GetObjective(tag);
                    this.UpdateStatus("Selected objective #" + tag.ToString() + ", hold down shift to start creating links, or press 'Ctrl+X' to delete");
                
                    // bind the property grid
                    this.ObjectiveProperties.SelectedObject = RenderHelper.SelectedObjective;
                }
                else if (this.linking)
                {
                    if (tag != RenderHelper.SelectedObjective.Id)
                    {
                        // this is our second item, the first becomes a prereq of the second
                        var obj = this.mission.GetObjective(tag);

                        // Fix link ambient zone
                        if (obj.GetType() != typeof(AmbientZone))
                        {
                            obj.AddPrerequisite(RenderHelper.SelectedObjective.Id);
                            this.UpdateStatus("Set objective #" + RenderHelper.SelectedObjective.Id.ToString() + " as prereq for objective #" + tag.ToString());
                            Log.Debug("Linked objective {0} to {1}", RenderHelper.SelectedObjective.Id, tag);
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
            var topPath = FileHelper.GetUsefulParentDirectory(this.loadedPath);

            if (topPath.Length > 0)
            {
                var dir = System.IO.Path.GetDirectoryName(topPath);
                diag.SelectedPath = dir; 
            }

            if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;

            var parts = diag.SelectedPath.Split('.');
            if (parts.Length == 0 || 
                !MapDefinitions.MapAliases.Contains(parts.Last())) 
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

            if (!File.Exists(missionPath))
            {
                Log.Warn("  - mission_data.json doesn't exist");
                var res = await this.ShowMessageAsync("No Mission Exists",
                    "This doesn't appear to be a properly formatted Anvil Framework mission. Would you like to create a new one at this location? " + Environment.NewLine + Environment.NewLine +
                    "WARNING: Doing this may overwrite parts of your mission.sqm.  Take a back up first", MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings() { NegativeButtonText = "No" });

                if (res == MessageDialogResult.Negative)
                {
                    Log.Debug("  - User aborted mission loading");
                    return;
                }

                Log.Debug("  - User requested a new mission to be created in this folder");

                // check that the folder name ends in the map alias
                var mapExtension = this.loadedPath.Split('.').Last();
                
                if (!MapDefinitions.Maps.ContainsKey(mapExtension))
                {
                    this.NewButtonClick(new object(), new RoutedEventArgs());
                    return;
                }

                var path = this.loadedPath;
                this.GenerateNewMission(mapExtension);
                this.loadedPath = path;
                this.SaveMission(this, new RoutedEventArgs());
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

            RenderHelper.SelectedObjective = null;
            this.ObjectiveProperties.SelectedObject = this.mission;

            this.RefreshScripts();

            this.mission.SQM = FileHelper.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

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

                // check that the folder name ends in the map alias
                var map = MapDefinitions.Maps.FirstOrDefault(o => o.Value.ImageName == this.mission.ImageName).Value;

                if (map == null)
                {
                    Log.Warn("Unknown map, aborting save");
                    return;
                }

                if (this.loadedPath.Split('.').Last() != map.MapAlias)
                {
                    var result = MessageBox.Show("The supplied folder name doesn't end in the expected map alias - " + map.MapAlias + " - if you continue you will " +
                        "be unable to open the map in the ArmA editor. Do you want to continue?", "Invalid Folder Name", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }

            this.SaveScriptSelection();

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            var savePath = System.IO.Path.Combine(this.loadedPath, "mission_data.json");

            // handle cases of users renaming the folder whilst editing (github #32)
            if (!Directory.Exists(this.loadedPath))
            {
                MessageBox.Show("Anvil couldn't find the mission directory for saving out the files. The editor has recreated the mission directory and exported the latest mission description file at the following path: " +
                    Environment.NewLine + Environment.NewLine + this.loadedPath + Environment.NewLine + Environment.NewLine + "This can be caused by renaming the mission folder whilst Anvil is open.", 
                    "Unable to find the mission directory?", MessageBoxButton.OK, MessageBoxImage.Error);
                Directory.CreateDirectory(this.loadedPath);
            }
            
            using (var sw = new StreamWriter(savePath))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this.mission);
                }
            }
            
            this.mission.SQM = FileHelper.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

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
            Log.Debug("Deleting objective ID {0}", RenderHelper.SelectedObjective.Id);

            var t = RenderHelper.SelectedObjective.GetType();
            if (t == typeof(Objective))
            {
                Log.Debug("  - Deleting objective");
                this.mission.DeleteObjective((Objective)RenderHelper.SelectedObjective);
            } 
            else if (t == typeof(AmbientZone))
            {
                Log.Debug("  - Deleting ambient zone");
                this.mission.DeleteAmbientZone(RenderHelper.SelectedObjective as AmbientZone);
            }
            else 
            {
                Log.Warn("  - Ignoring unknown deletion type");
                return;
            }

            RenderHelper.SelectedObjective = null;
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
            FileHelper.EmptyMissionDirectory(this.loadedPath);
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
            
            await CheckForEmptyAmmoboxAndApplyDefault();

            // check we have all the included scripts we require
            var missingScriptFolders = FileHelper.GetMissingIncludedScriptFolders(this.mission.IncludedScripts, this.mission.AvailableScripts);
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
                    Process.Start(System.IO.Path.Combine(FileHelper.GetFrameworkSourceFolder, "fw_scripts"));

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
            var src = FileHelper.GetFrameworkSourceFolder + System.IO.Path.DirectorySeparatorChar.ToString();
            Log.Debug("  - Copying mission files from {0}", src);
            FileHelper.SafeDirectoryCopy(src, this.loadedPath);

            if (!File.Exists(System.IO.Path.Combine(this.loadedPath, "mission_data.json")))
            {
                Log.Debug("  - Creating mission_data.json file");
                this.SaveMission(new object(), new RoutedEventArgs());
                Log.Debug("  - Done");
            }

            // edit the files
            Log.Debug("  - Creating output generator");
            var generator = new OutputHelper(this.mission);
            generator.Export(this.loadedPath);

            // read in the mission SQM file
            this.mission.SQM = FileHelper.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

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
                this.MapListBox.Items.Clear();

                // load up the mission names
                foreach (var map in MapDefinitions.Maps)
                {
                    var imagePath = System.IO.Path.Combine(FileHelper.GetDataFolder, "maps", map.Value.ImageName);
                    map.Value.IsDownloaded = System.IO.File.Exists(imagePath);
                    this.MapListBox.Items.Add(map.Key);
                }

                // select the first item
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

            RenderHelper.SelectedObjective = null;
            RenderHelper.ImageX = 0;
            RenderHelper.ImageY = 0;
            RenderHelper.ImageZoom = 2;
            this.loadedPath = "";

            this.mission = new Mission(this.DefaultAmmoboxContents);
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
            this.UpdateStatus("X: " + x + ", Y: " + y); // + "   [" + pos.X.ToString() + "," + pos.Y.ToString()+"]");
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
            this.mission.SQM = FileHelper.BuildSqmTreeFromFile(System.IO.Path.Combine(this.loadedPath, "mission.sqm"));

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
            RenderHelper.ZoomImage(e.Delta > 0, e.GetPosition(this), e.GetPosition(this.ObjectiveCanvas));
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
            var scriptPath = System.IO.Path.Combine(FileHelper.GetDataFolder, "supported_scripts.json");
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
        /// Update the credits box and the buttons which allow selecting or downloading the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MapListBox.SelectedValue == null)
            {
                this.MapDetailsTextBox.Text = "";
                this.MapSelectButton.IsEnabled = false;
                this.DownloadMapImageButton.IsEnabled = false;
            }
            else
            {
                var map = MapDefinitions.Maps[this.MapListBox.SelectedValue.ToString()];
                this.MapDetailsTextBox.Text = map.ToString();
                this.DownloadMapImageButton.IsEnabled = map.IsDownloadable;
                this.MapSelectButton.IsEnabled = map.IsDownloaded;
            }
        }

        /// <summary>
        /// Double click a map name to create a new map there immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.MapListBox.SelectedValue == null)
            {
                return;
            }

            var map = MapDefinitions.Maps[this.MapListBox.SelectedValue.ToString()];
            if (!map.IsDownloaded)
            {
                return;
            }

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
        /// Triggers the download of a new map image to the data folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadMapImageButtonClick(object sender, RoutedEventArgs e)
        {
            // check for unlikely edge cases - i.e. no selected item
            if (this.MapListBox.SelectedValue == null)
            {
                return;
            }

            var map = MapDefinitions.Maps[this.MapListBox.SelectedValue.ToString()];

            if (map.DownloadUrl == string.Empty)
            {
                await this.ShowMessageAsync("Unable to download map image", "Unable to download the map as no URL is available");
            }

            // do the download
            var mapFile = map.DownloadUrl.Split('/').Last();
            var mapDir = System.IO.Path.Combine(FileHelper.GetDataFolder, "maps");
            var savePath = System.IO.Path.Combine(mapDir, mapFile);

            // check if the map data folder exists and create it if not
            if (!Directory.Exists(mapDir))
            {
                Directory.CreateDirectory(mapDir);
            }

            // try downloading the map
            var success = false;
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(map.DownloadUrl, savePath);
                    success = true;
                }
            }
            catch (WebException ex)
            {
                Log.Error("  - Unable to download map from {0}", map.DownloadUrl);
                Log.Error("  - The error message was: {0}", ex.Message);
            }

            // handle success / failure
            if (success)
            {
                this.DownloadMapImageButton.IsEnabled = false;
                this.MapSelectButton.IsEnabled = true;
            }
            else
            {
                await this.ShowMessageAsync("Unable to download map image", 
                    "There was an error downloading the map image. Please check the URL works and contact the developer on the BI forums if the issue persists");
            }
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
                RenderHelper.SelectedObjective = obj;
                RenderHelper.ImageX = obj.ScreenX;
                RenderHelper.ImageY = obj.ScreenY;
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
        /// Opens up the help web site in the default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHelpPages(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.anvilproject.com/help/index.html");
        }

        /// <summary>
        /// Shows the ammobox generation dialog so that class names and quantities can be set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDefaultAmmoboxContentsDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var diag = new AmmoBoxContentsWindow(this.DefaultAmmoboxContents);
            var result = diag.ShowDialog();

            if (result == true)
            {
                this.DefaultAmmoboxContents = diag.Items.ToList();
                var ammoboxPath = System.IO.Path.Combine(FileHelper.GetDataFolder, "default_ammobox.json");
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Formatting = Formatting.Indented;

                using (var sw = new StreamWriter(ammoboxPath))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, this.DefaultAmmoboxContents);
                    }
                }
            }
        }

        /// <summary>
        /// Shows a dialog for editing the ammobox contents on the current mission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShowMissionAmmoboxContentsDialog(object sender, ExecutedRoutedEventArgs e)
        {
            // handle missing ammobox contents
            await CheckForEmptyAmmoboxAndApplyDefault();

            var diag = new AmmoBoxContentsWindow(this.mission.AmmoboxContents);
            var result = diag.ShowDialog();

            if (result == true)
            {
                this.mission.AmmoboxContents = diag.Items.ToList();
            }
        }

        /// <summary>
        /// Handle older missions which may not have ammobox defaults by asking if they would like to apply the defaults
        /// </summary>
        /// <returns></returns>
        private async System.Threading.Tasks.Task CheckForEmptyAmmoboxAndApplyDefault()
        {
            if (this.mission.AmmoboxContents == null)
            {
                var checkResult = await this.ShowMessageAsync("Misssing Ammobox Configuration", "This mission doesn't appear to have an ammobox set up, would you like to apply your default ammobox configuration?",
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = "No" });
                if (checkResult == MessageDialogResult.Affirmative)
                {
                    this.mission.SetAmmoboxContents(this.DefaultAmmoboxContents);
                }
                else
                {
                    this.mission.SetAmmoboxContents(new List<AmmoboxItem>());
                }
                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Restores the mission ammobox contents to the global defaults
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertMissionAmmoboxToDefault(object sender, ExecutedRoutedEventArgs e)
        {
            this.mission.SetAmmoboxContents(this.DefaultAmmoboxContents);
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
            e.CanExecute = this.selectionMode && RenderHelper.SelectedObjective != null;
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
