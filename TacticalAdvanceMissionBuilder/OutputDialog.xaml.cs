﻿using System;
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

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Interaction logic for OutputDialog.xaml
    /// </summary>
    public partial class OutputDialog : Window
    {
        /// <summary>
        /// Holds the mission which all data is generated from
        /// </summary>
        private readonly Mission mission;

        /// <summary>
        /// Holds the initialisation information
        /// </summary>
        private string init;

        /// <summary>
        /// Holds the markers information
        /// </summary>
        private string markers; 

        public OutputDialog(Mission mission)
        {
            InitializeComponent();

            this.mission = mission;

            this.Generate();

            this.MarkerText.Text = this.markers;
            this.InitText.Text = this.init;
        }

        /// <summary>
        /// Generates the output strings from the given mission
        /// </summary>
        private void Generate()
        {
            this.BuildInit();
            this.BuildMarkers();
        }

        /// <summary>
        /// Builds the script that should go into the framework/framework_init.sqf file
        /// </summary>
        private void BuildInit()
        {
            this.init = "objective_list = [\n";

            var lines = new List<string>();

            foreach (var obj in this.mission.Objectives)
            {
                lines.Add(String.Format(
                    "\t[{0,4}, {1,25}, {2,15}, {3,4}, {4,3}, {5,3}, {6,3}, {7,3}, {8,3}, {9,6}, {10,10}, {11,10}, {12,15}, {13}]",
                    obj.Id, "\"" + obj.Description + "\"", "\"" + this.mission.ObjectiveMarkerPrefix + "_" + obj.Id + "\"",
                    obj.Radius, obj.Infantry, obj.Motorised, obj.Armour, obj.Air, obj.TroopStrength, obj.NewSpawn ? "TRUE" : "FALSE",
                    "\"" + obj.AmmoMarker + "\"", "\"" + obj.SpecialMarker + "\"", 
                    "[" + (obj.Prerequisites.Count == 0 ? "FW_NONE" : string.Join(",", obj.Prerequisites.Select(x => x.ToString()).ToArray())) + "]", 
                    "\"" + obj.RewardDescription + "\"")
                );
            }

            this.init += string.Join(",\n", lines.ToArray());
            this.init += @"
];
publicVariable 'objective_list';";
        }

        /// <summary>
        /// Builds the marker script that should go into the mission.sqm file under the marker's section
        /// </summary>
        private void BuildMarkers()
        {
            var idx = this.mission.ObjectiveMarkerOffset;
            var markerCount = this.mission.ObjectiveMarkerOffset + this.mission.Objectives.Count();

            this.markers = "";

            foreach (var obj in this.mission.Objectives) 
            {
                this.markers += this.CreateMarker(idx, obj.X, obj.Y, this.mission.ObjectiveMarkerPrefix + "_" + obj.Id.ToString());
                idx++;

                if (obj.AmmoMarker != null && obj.AmmoMarker.Length > 0)
                {
                    this.markers += this.CreateMarker(idx, obj.X + 1, obj.Y + 1, obj.AmmoMarker, "ColorWest");
                    markerCount++;
                    idx++;
                }

                if (obj.SpecialMarker != null && obj.SpecialMarker.Length > 0)
                {
                    this.markers += this.CreateMarker(idx, obj.X - 1, obj.Y - 1, obj.SpecialMarker, "ColorWest");
                    markerCount++;
                    idx++;
                }
            }

            // prepend the marker count
            this.markers = string.Format("\titems = {0};\n{1}", markerCount, this.markers);
        }

        /// <summary>
        /// Creates a marker for the mission.sqm file
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="X">The x-coordinate of the marker</param>
        /// <param name="Y">The y-coordinate of the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <param name="color">The colour to use for the marker</param>
        /// <returns>The string of the marker object</returns>
        private string CreateMarker(int idx, double X, double Y, string name, string color)
        {
            var markers = "\tclass Item" + idx.ToString() + "\n\t{\n";
            markers += "\t\tposition[]={" + string.Format("{0:0.0}, 0, {1:0.0}", X, Y) + "};\n";
            markers += "\t\tname=\"" + name + "\";\n";
            markers += "\t\ttype=\"Empty\";\n\t\tcolorName=\"" + color + "\";\n\t};\n";
            return markers;
        }

        /// <summary>
        /// Creates an empty Orange marker
        /// </summary>
        /// <param name="idx">The ID to use for the marker</param>
        /// <param name="X">The x-coordinate of the marker</param>
        /// <param name="Y">The y-coordinate of the marker</param>
        /// <param name="name">The name of the marker</param>
        /// <returns>The string of the marker object</returns>
        private string CreateMarker(int idx, double X, double Y, string name)
        {
            return this.CreateMarker(idx, X, Y, name, "ColorOrange");
        }

        /// <summary>
        /// Closes the dialog when copying is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Selects all the text in a text box when the user clicks it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxClick(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.SelectAll();
        }
    }
}
