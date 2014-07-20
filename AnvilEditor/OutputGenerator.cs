using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using NLog;

using AnvilEditor.Models;
using AnvilEditor.Templates;

namespace AnvilEditor
{
    /// <summary>
    /// Generates mission output files for the given mission objects
    /// </summary>
    internal class OutputGenerator
    {
        /// <summary>
        /// Create a logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("OutputGenerator");

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

        /// <summary>
        /// Creates a new default instance of an OutputGenerator
        /// </summary>
        /// <param name="mission"></param>
        internal OutputGenerator(Mission mission)
        {
            Log.Debug("Starting OutputGenerator");
            this.mission = mission;

            this.BuildObjectiveList();
            this.BuildMissionData();
        }

        /// <summary>
        /// Builds the script that should go into the framework/framework_init.sqf file
        /// </summary>
        private void BuildObjectiveList()
        {
            Log.Debug("Creating objective list");
            
            this.objectiveList = "objective_list = [\n";

            var lines = new List<string>();

            foreach (var obj in this.mission.Objectives)
            {
                lines.Add(String.Format(
                    "\t[{0,4}, {1,30}, {2,15}, {3,4}, {4,3}, {5,3}, {6,3}, {7,3}, {8,3}, {9,6}, {10,10}, {11,15}, {12,20}, {13, 3}, {14}]",
                    obj.Id, "\"" + obj.Description + "\"", "\"" + this.mission.ObjectiveMarkerPrefix + "_obj_" + obj.Id + "\"",
                    obj.Radius, obj.Infantry, obj.Motorised, obj.Armour, obj.Air, obj.TroopStrength, obj.NewSpawn ? "TRUE" : "FALSE",
                    "\"" + (obj.Ammo ? this.mission.ObjectiveMarkerPrefix + "_" + obj.AmmoMarker : "") + "\"",
                    "\"" + (obj.Special ? this.mission.ObjectiveMarkerPrefix + "_" + obj.SpecialMarker : "")  + "\"",
                    "[" + (obj.Prerequisites.Count == 0 ? "FW_NONE" : string.Join(",", obj.Prerequisites.Select(x => x.ToString()).ToArray())) + "]",
                    obj.ObjectiveType, "\"" + obj.RewardDescription + "\"")
                );
            }

            this.objectiveList += string.Join(",\n", lines.ToArray());
            this.objectiveList += @"
];
publicVariable 'objective_list';";

            Log.Debug("  - Created {0} objectives", this.mission.Objectives.Count);
        }

        /// <summary>
        /// Builds mission data including ambient EOS spawns and sides
        /// </summary>
        private void BuildMissionData()
        {
            Log.Debug("Building Mission Data");
            this.missionData = @"enemyTeam = " + this.mission.EnemySide + @";
publicVariable ""enemyTeam"";" + Environment.NewLine;
            this.missionData += @"friendlyTeam = " + this.mission.FriendlySide + @";
publicVariable ""friendlyTeam"";" + Environment.NewLine;
            this.missionData += @"deleteTasks = " + (this.mission.DeleteTasks ? "1" : "0") + @"; 
publicVariable ""deleteTasks"";" + Environment.NewLine + Environment.NewLine;

            this.missionData += this.BuildAmbientSpawns();
        }

        /// <summary>
        /// Builds the script (placed in MissionData) for ambient spawn zones
        /// </summary>
        /// <returns></returns>
        private string BuildAmbientSpawns()
        {
            Log.Debug("Building ambient spawn scripts");
            var tpl = "_null = [[\"{0}\"],[{1},1],[{1},1,50],[{2},1],[{3},60],[0],[{4},0,50],[0, 1, 1000, {5}, FALSE, FALSE, [nil, FW_fnc_NOP]]] call EOS_Spawn;" + Environment.NewLine;
            var spawns = "";
            var i = 0;

            foreach (var spawn in this.mission.AmbientZones)
            {
                spawns += string.Format(tpl, 
                    this.mission.ObjectiveMarkerPrefix + "_amb_" + i.ToString(),
                    spawn.Infantry,
                    spawn.Motorised,
                    spawn.Armour,
                    spawn.Air,
                    this.mission.EnemySide
                );
                i++;
            }

            Log.Debug("Done building ambient spawn scripts");
            return spawns;
        }

        /// <summary>
        /// Exports a complete mission to the given folder path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal void Export(string path)
        {
            Log.Debug("Starting mission export");

            // export the mission parameters
            Log.Debug("  - Replacing mission_description.sqf data");
            var fwi = System.IO.Path.Combine(path, "framework", "mission_description.sqf");
            FileUtilities.ReplaceSection(fwi, "/* START OBJECTIVE LIST */", "/* END OBJECTIVE LIST */", this.ObjectiveList);
            FileUtilities.ReplaceSection(fwi, "/* START MISSION DATA */", "/* END MISSION DATA */", this.MissionData);

            // update and write the mission SQM
            Log.Debug("  - Updating mission.sqm");
            var mis = System.IO.Path.Combine(path, "mission.sqm");
            this.mission.UpdateSQM();
            using (var f = new StreamWriter(mis))
            {
                f.WriteLine(this.mission.SQM.ToSQM());
            }

            // then export and implement the required scripts
            Log.Debug("  - Including scripts");
            var script_init = "";
            var ext_init = "";
            var ext_fn = "";

            // Add included script initialisation and copy the files
            foreach (var included in this.mission.IncludedScripts)
            {
                ScriptInclude script = null;
                try {
                    script = this.mission.AvailableScripts.Where(o => o.FriendlyName == included).First();
                } 
                catch (ArgumentNullException) 
                {
                    Log.Warn("    * Unable to find script {0}", included);
                    MessageBox.Show("Unable to find script - '" + included + "', skipping");
                }

                if (script != null) {
                    Log.Debug("    * Copying script " + script.FriendlyName);
                    script_init += script.Init + Environment.NewLine;
                    ext_init += script.DescriptionExtInit + Environment.NewLine;
                    ext_fn  += script.DescriptionExtFunctions + Environment.NewLine;

                    // copy the directory
                    if (script.FolderName != "")
                    {
                        var src_path = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                            "mission_raw", "fw_scripts", script.FolderName);
                        var dst_path = System.IO.Path.Combine(path, script.FolderName);
                        FileUtilities.SafeDirectoryCopy(src_path, dst_path);
                    }
                }
            }

            Log.Debug("  - Writing init and description.ext code for scripts");
            var ext = System.IO.Path.Combine(path, "description.ext");
            var ini = System.IO.Path.Combine(path, "init.sqf");
            FileUtilities.ReplaceSection(ext, "/* START SCRIPT INIT */", "/* END SCRIPT INIT */", ext_init);
            FileUtilities.ReplaceSection(ext, "/* START SCRIPT FNS */", "/* END SCRIPT FNS */", ext_fn);
            FileUtilities.ReplaceSection(ini, "/* START ADDITIONAL SCRIPTS */", "/* END ADDITIONAL SCRIPTS */", script_init);

            // describe the mission in the description.ext
            Log.Debug("  - Changing mission description in description.ext");
            FileUtilities.ReplaceLines(ext, "OnLoadName = ", "OnLoadName = \"" + this.mission.MissionName + "\";");
            FileUtilities.ReplaceLines(ext, "OnLoadMission = ", "OnLoadMission = \"" + this.mission.MissionDescription + "\";");
            FileUtilities.ReplaceLines(ext, "enableDebugConsole = ", "enableDebugConsole = " + this.mission.DebugConsole + ";");
            FileUtilities.ReplaceLines(ext, "author = ", "author = \"" + this.mission.MissionAuthor + "\";");
        }

        /// <summary>
        /// Performs checks on the provided mission and objectives and returns error messages as a string
        /// </summary>
        /// <returns></returns>
        internal static string CompleteChecks(Mission mission)
        {
            string result = string.Empty; 

            foreach (var obj in mission.Objectives)
            {
                if (obj.IgnoreOverOccupation) continue;

                // check for over dense objectives
                var mass = obj.Infantry * (obj.TroopStrength + 1) +
                            obj.Motorised * (obj.TroopStrength + 1) +
                            obj.Armour * (obj.TroopStrength + 1);
                float density = mass / (float)obj.Radius;

                if (density > 0.2)
                {
                    result += "WARNING: Occupation of objective " + obj.Id.ToString() + " is relatively heavy. " + 
                        "You may wish to consider reducing the troop strength, number of units or increasing the radius" +
                        Environment.NewLine;
                }
            }

            if (mission.FriendlySide == mission.EnemySide)
            {
                result += "ERROR: The friendly and enemy side are the same... I have no idea what will happen but it can't be good" + Environment.NewLine;
            }

            var emptyCount = mission.Objectives.Where(o => !o.IsOccupied).Count();
            if (emptyCount > 0)
            {
                result += "WARNING: There are " + emptyCount.ToString() + " unoccupied objective(s)" + Environment.NewLine;
            }

            emptyCount = mission.AmbientZones.Where(o => !o.IsOccupied).Count();
            if (emptyCount > 0)
            {
                result += "WARNING: There are " + emptyCount.ToString() + " unoccupied ambient zone(s)" + Environment.NewLine;
            }


            return result;
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
