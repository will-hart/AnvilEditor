using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Holds objective data that can be written to file
    /// </summary>
    public class Objective
    {
        /// <summary>
        /// The minimum X map coordinate for the given map image
        /// </summary>
        private static double MapXMin = 2000;

        /// <summary>
        /// The maximum X map coordinate for the given map image
        /// </summary>
        private static double MapXMax = 30000;

        /// <summary>
        /// The minimum Y map coordinate for the given map image
        /// </summary>
        private static double MapYMin = 5000;

        /// <summary>
        /// The maximum Y map coordinate for the given map image
        /// </summary>
        private static double MapYMax = 26000;

        /// <summary>
        /// The unscaled X size of the map image control
        /// </summary>
        private static double ScreenXMax = 800;

        /// <summary>
        /// The unscaled Y size of the map image control
        /// </summary>
        private static double ScreenYMax = 600;

        /// <summary>
        /// A list of prereq ID numbers to unlock this objective
        /// </summary>
        private readonly List<int> prereqs = new List<int>();

        /// <summary>
        /// The x-coordinate in screen coordinates
        /// </summary>
        private double screenX;

        /// <summary>
        /// The y-coordinate in screen coordinates
        /// </summary>
        private double screenY;

        /// <summary>
        /// Default constructor, creates a new objective with the given id
        /// </summary>
        /// <param name="id">The ID number to use to refer to this objective</param>
        public Objective(int id, Point location)
        {
            this.Id = id;
            this.screenX = location.X;
            this.screenY = location.Y;

            // set some defaults
            this.Radius = 50;
            this.Infantry = 0;
            this.Motorised = 0;
            this.Armour = 0;
            this.Air = 0;
            this.TroopStrength = 0;
        }

        /// <summary>
        /// Adds a prerequisite objective if it doesn't already exist
        /// </summary>
        /// <param name="id">The id of the prerequisite objective</param>
        public void AddPrerequisite(int id)
        {
            if (!this.prereqs.Contains(id))
            {
                this.prereqs.Add(id);
            }
        }

        /// <summary>
        /// Converts a map x co-ordinate to a canvas x-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The canvas x co-ordinate</returns>
        internal static double MapToCanvasX(double value)
        {
            return ScreenXMax * ((value - MapXMin) / (MapXMax - MapXMin));
        }

        /// <summary>
        /// Converts a canvas x co-ordinate to a map x-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The map x co-ordinate</returns>
        internal static double CanvasToMapX(double value)
        {
            return MapXMin + (value / ScreenXMax) * (MapXMax - MapXMin);
        }

        /// <summary>
        /// Converts a map y co-ordinate to a canvas y-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The canvas y co-ordinate</returns>
        internal static double MapToCanvasY(double value)
        {
            return ScreenYMax * (1 - (value - MapYMin) / (MapYMax - MapYMin));
        }

        /// <summary>
        /// Converts a canvas y co-ordinate to a map y-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The map y co-ordinate</returns>
        internal static double CanvasToMapY(double value)
        {
            return MapYMax - (value / ScreenYMax) * (MapYMax - MapYMin);
        }

        /// <summary>
        /// Generates the init text for this objective
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>The text that should be included in the framework_init.sqf file for this objective</returns>
        internal string GetInitText(string prefix)
        {
            return String.Format(
                    "\t[{0,4}, {1,30}, {2,15}, {3,4}, {4,3}, {5,3}, {6,3}, {7,3}, {8,3}, {9,6}, {10,10}, {11,15}, {12,20}, {13, 3}, {14}]",
                    this.Id, "\"" + this.Description + "\"", "\"" + prefix + "_" + this.Id + "\"",
                    this.Radius, this.Infantry, this.Motorised, this.Armour, this.Air, this.TroopStrength, this.NewSpawn ? "TRUE" : "FALSE",
                    "\"" + this.AmmoMarker + "\"", "\"" + this.SpecialMarker + "\"",
                    "[" + (this.Prerequisites.Count == 0 ? "FW_NONE" : string.Join(",", this.Prerequisites.Select(x => x.ToString()).ToArray())) + "]",
                    this.ObjectiveType, "\"" + this.RewardDescription + "\"");
        }



        /// <summary>
        /// Creates a marker for the mission.sqm file
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <param name="color">The colour to use for the marker</param>
        /// <param name="text">The text to display for the marker on the map</param>
        /// <returns>The string of the marker object</returns>
        internal string CreateMarker(int idx, string name, string color, string text)
        {
            var markers = "\t\tclass Item" + idx.ToString() + "\n\t\t{\n";
            markers += "\t\t\tposition[]={" + string.Format("{0:0.0}, 0, {1:0.0}", this.X, this.Y) + "};\n";
            markers += "\t\t\tname=\"" + name + "\";\n";
            markers += "\t\t\ttype=\"Empty\";\n\t\t\tcolorName=\"" + color + "\";";

            if (text.Length > 0)
            {
                markers += "\n\t\t\ttext = \"" + text + "\";";
            }

            markers += "\n\t\t};\n";
            return markers;
        }

        /// <summary>
        /// Creates a marker for the mission.sqm file
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <param name="color">The colour to use for the marker</param>
        /// <returns>The string of the marker object</returns>
        internal string CreateMarker(int idx, string name, string color)
        {
            return this.CreateMarker(idx, name, color, "");
        }

        /// <summary>
        /// Creates an empty Orange marker
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <returns>The string of the marker object</returns>
        internal string CreateMarker(int idx, string name)
        {
            return this.CreateMarker(idx, name, "ColorOrange", "");
        }

        /// <summary>
        /// Gets a value which is the internal ID of this mission
        /// </summary>
        [Category("Details")]
        [Description("The ID of the objective (readonly)"), ReadOnly(true)]
        public int Id { get; set;  }

        /// <summary>
        /// Gets or sets a value used to describe this objective
        /// </summary>
        [Category("Details")]
        [Description("The description that will appear in the in-game task")]
        public string Description { get; set; }

        [Category("Location")]
        [Description("The X coordinate for this objective")]
        public double X 
        {
            get
            {
                return CanvasToMapX(this.screenX);
            }
            set
            {
                this.screenX = MapToCanvasX(value);
            }
        }

        [Category("Location")]
        [Description("The Y coordinate for this objective")]
        public double Y
        {
            get
            {
                return CanvasToMapY(this.screenY);
            }
            set
            {
                this.screenY = MapToCanvasY(value);
            }
        }

        [Category("Location")]
        [DisplayName("Objective Radius")]
        [Description("The area that will be occupied by enemy forces (in meters)")]
        public int Radius { get; set; }

        [Category("Strength")]
        [Description("The number of infantry units in the area")]
        public int Infantry { get; set; }

        [Category("Strength")]
        [Description("The number of motorised units in the area")]
        public int Motorised { get; set; }

        [Category("Strength")]
        [Description("The number of armour units in the area")]
        public int Armour { get; set; }

        [Category("Strength")]
        [Description("The number of air units in the area")]
        public int Air { get; set; }

        [Category("Strength")]
        [Description("The overall strength multiplier of the area")]
        public int TroopStrength { get; set; }

        [Category("Rewards")]
        [DisplayName("Create new spawn")]
        [Description("Set to true if a new spawn point should be created at this objective once captured")]
        public bool NewSpawn { get; set; }

        [Category("Rewards")]
        [DisplayName("Spawn Ammo Drop")]
        [Description("Should an ammo drop occur at this objective?")]
        public bool Ammo { get; set; }

        [Category("Rewards")]
        [DisplayName("Spawn Special Reward")]
        [Description("Should a special reward marker be dropped at this location")]
        public bool Special { get; set; }

        [Category("Details")]
        [Description ("A readonly list of objectives that must be achieved to unlock this one")]
        [Editor(typeof(CollectionEditor), typeof(CollectionEditor))]
        public List<int> Prerequisites { get { return this.prereqs; } }

        [Category("Rewards")]
        [DisplayName("Reward Description")]
        [Description("Text that will be added to the in game objective showing the rewards that will be unlocked upon capturing this objective")]
        public string RewardDescription { get; set; }

        [Category("Details")]
        [DisplayName("Objective Type")]
        [Description("The type of mission that will be generated on the marker")]
        [ItemsSource(typeof(MissionTypeItemSource))]
        public int ObjectiveType { get; set; }

        /// <summary>
        /// The name of the ammo marker dropped at the objective location
        /// </summary>
        internal string AmmoMarker
        {
            get
            {
                return this.Ammo ? "ammo_" + this.Id.ToString() : "";
            }
        }

        /// <summary>
        /// The name of the special marker dropped at the objective location
        /// </summary>
        internal string SpecialMarker
        {
            get
            {
                return this.Special ? "special_" + this.Id.ToString() : "";
            }
        }

        /// <summary>
        /// Gets a value indicating where the objective should be drawn on screen in the X coordinate
        /// </summary>
        internal double ScreenX
        {
            get
            {
                return this.screenX;
            }
        }

        /// <summary>
        /// Gets a value indicating where the objective should be drawn on screen in the Y coordinate
        /// </summary>
        internal double ScreenY
        {
            get
            {
                return this.screenY;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the objective is occupied
        /// </summary>
        internal bool IsOccupied
        {
            get
            {
                return this.Air + this.Armour + this.Infantry + this.Motorised != 0;
            }
        }
    }
}
