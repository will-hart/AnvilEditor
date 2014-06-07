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
            this.missionData = @"enemyTeam = " + this.mission.EnemySide + @";
publicVariable ""enemyTeam"";" + Environment.NewLine + Environment.NewLine;

            this.missionData += this.BuildAmbientSpawns();
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

            // add the respawn marker
            this.markers += Objective.CreateMarker(this.mission.RespawnX, this.mission.RespawnY, idx, "respawn_west", "ColorWest", "respawn_west");
            idx++;
            markerCount++;

            var i = 0;
            foreach (var az in this.mission.AmbientZones)
            {
                this.markers += Objective.CreateMarker(az.X, az.Y, idx, this.mission.ObjectiveMarkerPrefix + "_ambient_" + i.ToString(), "ColorOrange", "AMB_" + i.ToString());
                idx++;
                markerCount++;
                i++;
            }

            // prepend the marker count
            this.markers = string.Format("\t\titems = {0};\n{1}", markerCount, this.markers);
        }

        /// <summary>
        /// Builds the script (placed in MissionData) for ambient spawn zones
        /// </summary>
        /// <returns></returns>
        private string BuildAmbientSpawns()
        {
            var tpl = "_null = [[\"{0}\"],[{1},1],[{1},1,50],[{2},1],[{3},60],[0],[{4},0,50],[0, 1, 1000, {5}, FALSE, FALSE, [[], FW_fnc_NOP]] call EOS_Spawn;" + Environment.NewLine;
            var spawns = "";
            var i = 0;

            foreach (var spawn in this.mission.AmbientZones)
            {
                spawns += string.Format(tpl, 
                    this.mission.ObjectiveMarkerPrefix + "_ambient_" + i.ToString(),
                    spawn.Infantry,
                    spawn.Motorised,
                    spawn.Armour,
                    spawn.Air,
                    this.mission.EnemySide
                );
                i++;
            }

            return spawns;
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
            FileUtilities.ReplaceSection(fwi, "/* START OBJECTIVE LIST */", "/* END OBJECTIVE LIST */", this.ObjectiveList);
            FileUtilities.ReplaceSection(fwi, "/* START MISSION DATA */", "/* END MISSION DATA */", this.MissionData);

            var mis = System.IO.Path.Combine(path, "mission.sqm");
            FileUtilities.ReplaceSection(mis, "/* START FRAMEWORK MARKERS */", "/* END FRAMEWORK MARKERS */", this.Markers);

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

                    // copy the directory
                    var src_path = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "mission_raw", "fw_scripts", script.FolderName);
                    var dst_path = System.IO.Path.Combine(path, "fw_scripts", script.FolderName);
                    FileUtilities.SafeDirectoryCopy(src_path, dst_path);
                }
            }

            var ext = System.IO.Path.Combine(path, "description.ext");
            var ini = System.IO.Path.Combine(path, "init.sqf");
            FileUtilities.ReplaceSection(ext, "/* START SCRIPT INIT */", "/* END SCRIPT INIT */", ext_init);
            FileUtilities.ReplaceSection(ext, "/* START SCRIPT FNS */", "/* END SCRIPT FNS */", ext_fn);
            FileUtilities.ReplaceSection(ini, "/* START ADDITIONAL SCRIPTS */", "/* END ADDITIONAL SCRIPTS */", script_init);
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
