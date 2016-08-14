﻿namespace AnvilEditor.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Windows;
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    using Sources;

    /// <summary>
    /// Holds objective data that can be written to file
    /// </summary>
    public class ObjectiveBase
    {
        /// <summary>
        /// The x-coordinate in screen coordinates
        /// </summary>
        protected double screenX;

        /// <summary>
        /// The y-coordinate in screen coordinates
        /// </summary>
        protected double screenY;

        /// <summary>
        /// Default constructor, creates a new objective with the given id
        /// </summary>
        /// <param name="id">The ID number to use to refer to this objective</param>
        public ObjectiveBase(int id, Point location) 
        {
        }

        /// <summary>
        /// Converts a map x co-ordinate to a canvas x-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The canvas x co-ordinate</returns>
        public static double MapToCanvasX(double value)
        {
            return MainWindow.ScreenXMax * ((value - MainWindow.MapXMin) / (MainWindow.MapXMax - MainWindow.MapXMin));
        }

        /// <summary>
        /// Converts a canvas x co-ordinate to a map x-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The map x co-ordinate</returns>
        public static int CanvasToMapX(double value)
        {
            return (int)(MainWindow.MapXMin + (value / MainWindow.ScreenXMax) * (MainWindow.MapXMax - MainWindow.MapXMin));
        }

        /// <summary>
        /// Converts a map y co-ordinate to a canvas y-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The canvas y co-ordinate</returns>
        public static double MapToCanvasY(double value)
        {
            return MainWindow.ScreenYMax * (1 - (value - MainWindow.MapYMin) / (MainWindow.MapYMax - MainWindow.MapYMin));
        }

        /// <summary>
        /// Converts a canvas y co-ordinate to a map y-coordinate
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The map y co-ordinate</returns>
        public static int CanvasToMapY(double value)
        {
            return (int)(MainWindow.MapYMax - (value / MainWindow.ScreenYMax) * (MainWindow.MapYMax - MainWindow.MapYMin));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <param name="color">The colour to use for the marker</param>
        /// <param name="text">The text to display for the marker on the map</param>
        public static string CreateMarker(int x, int y, int idx, string name, string color, string text) 
        {
            var markers = "\t\tclass Item" + idx.ToString() + "\n\t\t{\n";
            markers += "\t\t\tposition[]={" + string.Format("{0:0.0}, 0, {1:0.0}", x, y) + "};\n";
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
        /// <param name="text">The text to display for the marker on the map</param>
        /// <returns>The string of the marker object</returns>
        public string CreateMarker(int idx, string name, string color, string text)
        {
            return CreateMarker(this.X, this.Y, idx, name, color, text);
        }

        /// <summary>
        /// Creates a marker for the mission.sqm file
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <param name="color">The colour to use for the marker</param>
        /// <returns>The string of the marker object</returns>
        public string CreateMarker(int idx, string name, string color)
        {
            return CreateMarker(this.X, this.Y, idx, name, color, "");
        }

        /// <summary>
        /// Creates an empty Orange marker
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <returns>The string of the marker object</returns>
        public string CreateMarker(int idx, string name)
        {
            return CreateMarker(this.X, this.Y, idx, name, "ColorOrange", "");
        }
        
        /// <summary>
        /// Gets a value which is the internal ID of this mission
        /// </summary>
        [Category("Details")]
        [Description("The ID of the objective (readonly)"), ReadOnly(true)]
        public int Id { get; set; }

        [Category("Location")]
        [Description("The X coordinate for this objective")]
        [JsonConverter(typeof(ForceIntJsonConvertor))]
        public int X 
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
        [JsonConverter(typeof(ForceIntJsonConvertor))]
        public int Y
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
        [DisplayName("Infantry Groups")]
        [Description("The number of infantry units in the area")]
        [PropertyOrder(2)]
        public int Infantry { get; set; }

        [Category("Strength")]
        [DisplayName("Motorised Groups")]
        [Description("The number of motorised units in the area")]
        [PropertyOrder(3)]
        public int Motorised { get; set; }

        [Category("Strength")]
        [DisplayName("Armour Groups")]
        [Description("The number of armour units in the area")]
        [PropertyOrder(1)]
        public int Armour { get; set; }

        [Category("Strength")]
        [DisplayName("Air Groups")]
        [Description("The number of air units in the area")]
        [PropertyOrder(0)]
        public int Air { get; set; }

        [Category("Strength")]
        [DisplayName("Group Size")]
        [Description("The number of individuals in each of the groups above")]
        [ItemsSource(typeof(GroupSizeItemSource))]
        [PropertyOrder(4)]
        public int TroopStrength { get; set; }
        
        /// <summary>
        /// Gets a value indicating where the objective should be drawn on screen in the X coordinate
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
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
        [Browsable(false)]
        public bool IsOccupied
        {
            get
            {
                return this.Air + this.Armour + this.Infantry + this.Motorised != 0;
            }
        }
    }
}
