using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Generates mission output files for the given mission objects
    /// </summary>
    internal class OutputGenerator
    {
        /// <summary>
        /// Holds framework_init strings
        /// </summary>
        private string objectiveList;

        /// <summary>
        /// Holds marker strings
        /// </summary>
        private string markers;

        /// <summary>
        /// Holds additional mission data used in the mission_data.sqf file
        /// </summary>
        private string missionData;

        /// <summary>
        /// The mission being generated
        /// </summary>
        private Mission mission;

        internal OutputGenerator(Mission mission)
        {
            this.mission = mission;

            this.BuildObjectiveList();
            this.BuildMarkers();
            this.BuildMissionData();
        }

        /// <summary>
        /// Opens and edits the given file and replaces the MARKER with the text of REPLACEWITH
        /// </summary>
        /// <param name="path">The path of the file to edit</param>
        /// <param name="markerStart">The marker to replace from</param>
        /// <param name="markerEnd">The marker to replace until</param>
        /// <param name="replaceWith">The text to replace the marker with</param>
        internal static void ReplaceSection(string path, string markerStart, string markerEnd, string replaceWith)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var newLines = new List<string>();
            bool? found = null;

            // a regex.replace would probably be better but then ... regex

            foreach (var line in lines)
            {
                // if we are within the markers, don't append the line
                if (found == false)
                {
                    if (line.Contains(markerEnd))
                    {
                        newLines.Add(line);
                        found = true;
                    }
                }
                else
                {
                    newLines.Add(line);

                    if (found == null && line.Contains(markerStart))
                    {
                        found = false;
                        newLines.Add(replaceWith);
                    }
                }
            }

            System.IO.File.WriteAllLines(path, newLines);
        }

        /// <summary>
        /// Replaces all lines in the file starting with `lineStart` with the value in `replaceWith`
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="lineStart">The start of the line to match</param>
        /// <param name="replaceWith">The value to replace with</param>
        internal static void ReplaceLines(string path, string lineStart, string replaceWith)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith(lineStart))
                {
                    newLines.Add(replaceWith);
                }
                else
                {
                    newLines.Add(line);
                }
            }

            System.IO.File.WriteAllLines(path, newLines);
        }

        /// <summary>
        /// Copy the raw mission files to the given directory and edit the
        /// framework_init and mission SQM files to add in the generated content
        /// 
        /// Borrowed some code from http://stackoverflow.com/a/12283793/233608
        /// 
        /// This is called "safe" as it does not overwrite the mission.sqm file,
        /// only updates the contents between the markers
        /// </summary>
        /// <param name="dest">The destination root directory</param>
        internal static void SafeDirectoryCopy(string src, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            if (Path.GetFileName(dest) == "fw_scripts") return;

            var dirInfo = new DirectoryInfo(src);
            var files = dirInfo.GetFiles();

            foreach (var tempfile in files)
            {
                var path = System.IO.Path.Combine(dest, tempfile.Name);
                if (tempfile.Name == "mission.sqm")
                {
                    try
                    {
                        tempfile.CopyTo(path);
                    }
                    catch (IOException e)
                    {
                        // squash if it is an "already exists" exception
                        if (!e.Message.Contains("already exists")) throw;

                        // otherwise check if we want to overwrite the mission.sqm
                        var res = MessageBox.Show(
                            "Do you want to overwrite the mission.sqm file (Click YES) or manually paste in new markers (Click NO)? IF you select yes you will lose any changes you made in the editor. You can use the 'preview' functionality to see the marker text",
                            "Overwrite mission file?",
                            MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes) tempfile.CopyTo(path, true);
                    }
                }
                else
                {
                    tempfile.CopyTo(path, true);
                }
            }

            var dirs = dirInfo.GetDirectories();
            foreach (var tempdir in dirs)
            {
                SafeDirectoryCopy(
                    System.IO.Path.Combine(src, tempdir.Name), System.IO.Path.Combine(dest, tempdir.Name));
            }
        }

        /// <summary>
        /// Builds the script that should go into the framework/framework_init.sqf file
        /// </summary>
        private void BuildObjectiveList()
        {
            this.objectiveList = "objective_list = [\n";

            var lines = new List<string>();

            foreach (var obj in this.mission.Objectives)
            {
                lines.Add(String.Format(
                    "\t[{0,4}, {1,30}, {2,15}, {3,4}, {4,3}, {5,3}, {6,3}, {7,3}, {8,3}, {9,6}, {10,10}, {11,15}, {12,20}, {13, 3}, {14}]",
                    obj.Id, "\"" + obj.Description + "\"", "\"" + this.mission.ObjectiveMarkerPrefix + "_" + obj.Id + "\"",
                    obj.Radius, obj.Infantry, obj.Motorised, obj.Armour, obj.Air, obj.TroopStrength, obj.NewSpawn ? "TRUE" : "FALSE",
                    "\"" + obj.AmmoMarker + "\"", "\"" + obj.SpecialMarker + "\"",
                    "[" + (obj.Prerequisites.Count == 0 ? "FW_NONE" : string.Join(",", obj.Prerequisites.Select(x => x.ToString()).ToArray())) + "]",
                    obj.ObjectiveType, "\"" + obj.RewardDescription + "\"")
                );
            }

            this.objectiveList += string.Join(",\n", lines.ToArray());
            this.objectiveList += @"
];
publicVariable 'objective_list';";
        }

        private void BuildMissionData()
        {
            this.missionData = @"enemyTeam = EAST;
publicVariable ""enemyTeam"";";
        }

        /// <summary>
        /// Builds the marker script that should go into the mission.sqm file under the marker's section
        /// </summary>
        private void BuildMarkers()
        {
            var idx = 0;
            var markerCount = this.mission.ObjectiveMarkerOffset + this.mission.Objectives.Count();

            this.markers = "";

            foreach (var obj in this.mission.Objectives)
            {
                this.markers += obj.CreateMarker(idx, this.mission.ObjectiveMarkerPrefix + "_" + obj.Id.ToString(), "ColorOrange", "OBJ_" + obj.Id.ToString());
                idx++;

                if (obj.AmmoMarker != null && obj.AmmoMarker.Length > 0)
                {
                    this.markers += obj.CreateMarker(idx, obj.AmmoMarker, "ColorWest", "AMMO");
                    markerCount++;
                    idx++;
                }

                if (obj.SpecialMarker != null && obj.SpecialMarker.Length > 0)
                {
                    this.markers += obj.CreateMarker(idx, obj.SpecialMarker, "ColorWest", "SPECIAL");
                    markerCount++;
                    idx++;
                }
            }

            // prepend the marker count
            this.markers = string.Format("\t\titems = {0};\n{1}", markerCount, this.markers);
        }

        /// <summary>
        /// Exports a complete mission to the given folder path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal void Export(string path)
        {
            // export the mission parameters
            var fwi = System.IO.Path.Combine(path, "framework", "mission_description.sqf");
            ReplaceSection(fwi, "/* START OBJECTIVE LIST */", "/* END OBJECTIVE LIST */", this.ObjectiveList);
            ReplaceSection(fwi, "/* START MISSION DATA */", "/* END MISSION DATA */", this.MissionData);

            var mis = System.IO.Path.Combine(path, "mission.sqm");
            ReplaceSection(mis, "/* START FRAMEWORK MARKERS */", "/* END FRAMEWORK MARKERS */", this.Markers);

            // then export and implement the required scripts
            var script_init = "";
            var ext_init = "";
            var ext_fn = "";

            foreach (var included in this.mission.IncludedScripts)
            {
                ScriptInclude script = null;
                try {
                    script = this.mission.AvailableScripts.Where(o => o.FriendlyName == included).First();
                } 
                catch (ArgumentNullException) 
                {
                    MessageBox.Show("Unable to find script - '" + included + "', skipping");
                }

                if (script != null) {
                    script_init += script.Init + Environment.NewLine;
                    ext_init += script.DescriptionExtInit + Environment.NewLine;
                    ext_fn  += script.DescriptionExtFunctions + Environment.NewLine;
                }
            }

            var ext = System.IO.Path.Combine(path, "description.ext");
            var ini = System.IO.Path.Combine(path, "init.sqf");
            ReplaceSection(ext, "/* START SCRIPT INIT */", "/* END SCRIPT INIT */", ext_init);
            ReplaceSection(ext, "/* START SCRIPT FNS */", "/* END SCRIPT FNS */", ext_fn);
            ReplaceSection(ini, "/* START ADDITIONAL SCRIPTS */", "/* END ADDITIONAL SCRIPTS */", script_init);
        }

        /// <summary>
        /// Gets a value containing framework_init data
        /// </summary>
        internal string ObjectiveList
        {
            get
            {
                return this.objectiveList;
            }
        }

        /// <summary>
        /// Gets a value containing marker information for the mission.sqm
        /// </summary>
        internal string Markers
        {
            get
            {
                return this.markers;
            }
        }

        /// <summary>
        /// Gets a value containing mission data, included in the mission_data.sqf below the objective list.
        /// </summary>
        internal string MissionData
        {
            get
            {
                return this.missionData;
            }
        }
    }
}
