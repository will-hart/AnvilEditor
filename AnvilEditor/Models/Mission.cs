using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

using AnvilEditor.Templates;

using AnvilParser;
using AnvilParser.Tokens;

namespace AnvilEditor.Models
{
    public class Mission
    {

        /// <summary>
        /// The minimum X map coordinate for the given map image
        /// </summary>
        public int MapXMin = 2000;

        /// <summary>
        /// The maximum X map coordinate for the given map image
        /// </summary>
        public int MapXMax = 30000;

        /// <summary>
        /// The minimum Y map coordinate for the given map image
        /// </summary>
        public int MapYMin = 5000;

        /// <summary>
        /// The maximum Y map coordinate for the given map image
        /// </summary>
        public int MapYMax = 26000;

        /// <summary>
        /// The name of the image used in this map
        /// </summary>
        public string ImageName = "altis.png";

        /// <summary>
        /// A list of objectives in the mission
        /// </summary>
        private readonly List<Objective> objectives = new List<Objective>();

        /// <summary>
        /// A list of scripts that can be included
        /// </summary>
        private readonly List<ScriptInclude> availableScripts = new List<ScriptInclude>();

        /// <summary>
        /// A list of included scripts
        /// </summary>
        private readonly List<string> includedScripts = new List<string>();

        /// <summary>
        /// A list of editor created ambient zones where enemy infantry occupy
        /// </summary>
        private readonly List<AmbientZone> ambientZones = new List<AmbientZone>();

        /// <summary>
        /// Holds the SQM tree for the mission.sqm file in the directory
        /// </summary>
        private ParserClass sqm = TemplateFactory.Mission();

        /// <summary>
        /// The next ID to use for objectives
        /// </summary>
        public int nextId = 0;

        /// <summary>
        /// A list of IDs that have become available for use
        /// </summary>
        public readonly List<int> availableIds = new List<int>();

        /// <summary>
        /// The prefix to put in front of objectives
        /// </summary>
        [Browsable(false)]
        public string ObjectiveMarkerPrefix { get; set; }

        [DisplayName("Delete Completed Tasks")]
        [Category("Details")]
        [Description("For larger missions, should completed tasks be deleted from the task list? Check the box to remove completed tasks, or leave it unchecked to leave completed tasks in the player's task list.")]
        public bool DeleteTasks { get; set; }

        /// <summary>
        /// The Item number in the mission SQM file to start counting objective markers from
        /// </summary>
        [Category("Details")]
        [DisplayName("Additional Markers")]
        [Description("The number of markers that come after this (added in the editor). WARNING - take care when adding markers in the editor then adding new objectives and regenerating, as some duplicate marker names may appear.")]
        public int ObjectiveMarkerOffset { get; set; }

        /// <summary>
        /// Creates a new mission, setting default properties and loading in the available scripts from file
        /// </summary>
        public Mission()
        {
            this.ObjectiveMarkerPrefix = "afw";
            this.ObjectiveMarkerOffset = 0;
            this.MissionName = "Anvil Mission";
            this.MissionDescription = "A mission made with the Anvil Framework";
            this.MissionAuthor = "Framework by |TG| Will";
            this.EnemySide = "EAST";
            this.FriendlySide = "WEST";
            this.DebugConsole = 0;
            this.DeleteTasks = false;
            this.EndTrigger = EndTriggerTypes.None;
            this.RandomObjectiveOrder = false;
            this.MissionBriefing = new Briefing();

            // load in the supported scripts
            var dataPath = System.IO.Path.Combine( 
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data");
            var scriptPath = System.IO.Path.Combine(dataPath, "supported_scripts.json");

            using (var sr = new StreamReader(scriptPath))
            {
                var json = sr.ReadToEnd();
                this.availableScripts = JsonConvert.DeserializeObject<List<ScriptInclude>>(json);

                this.availableScripts.Sort((a, b) => a.FriendlyName.CompareTo(b.FriendlyName));
            }
        }

        /// <summary>
        /// Get an objective by ID
        /// </summary>
        /// <param name="id">The id to return an objective for</param>
        /// <returns>An objective with the given Id or null</returns>
        internal Objective GetObjective(int id)
        {
            return this.objectives.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Deletes the objective with the given id
        /// </summary>
        /// <param name="id">The id of the objective to delete</param>
        internal void DeleteObjective(int id)
        {
            var obj = this.GetObjective(id);
            this.DeleteObjective(obj);
        }

        /// <summary>
        /// Deletes the objective object given
        /// </summary>
        /// <param name="obj">The objective object to delete</param>
        internal void DeleteObjective(Objective obj) 
        {
            this.objectives.Remove(obj);

            foreach (var o in this.objectives)
            {
                o.Prerequisites.Remove(obj.Id);
            }

            this.availableIds.Add(obj.Id);
            this.availableIds.Sort();
        }

        /// <summary>
        /// Clears a mission back to a new state
        /// </summary>
        internal Mission ClearMission()
        {
            return new Mission();
        }

        /// <summary>
        /// Adds a new objective to the mission
        /// </summary>
        /// <param name="location">The location of the objective</param>
        /// <returns>The objective that was just created</returns>
        internal Objective AddObjective(Point location)
        {
            var id = this.nextId;

            if (this.availableIds.Count > 0)
            {
                id = this.availableIds[0];
                this.availableIds.RemoveAt(0);
            }
            else
            {
                this.nextId++;
            }

            var obj = new Objective(id, location);
            this.objectives.Add(obj);

            this.objectives.Sort((a, b) => a.Id.CompareTo(b.Id));

            return obj;
        }

        /// <summary>
        /// Applies the scripts to be used as given in the editor
        /// </summary>
        /// <param name="list"></param>
        internal void UseScript(string script)
        {
            if (!this.includedScripts.Contains(script))
            {
                this.includedScripts.Add(script);
            }
        }

        /// <summary>
        /// Removes a script name from the included scripts collection
        /// </summary>
        /// <param name="script"></param>
        internal void RemoveScript(string script)
        {
            this.includedScripts.Remove(script);
        }

        /// <summary>
        /// Sets the initial respawn point of the mission
        /// </summary>
        /// <param name="pos"></param>
        internal void SetRespawn(Point pos)
        {
            this.RespawnX = Objective.CanvasToMapX(pos.X);
            this.RespawnY = Objective.CanvasToMapY(pos.Y);
        }

        /// <summary>
        /// Create a new ambient zone and return it
        /// </summary>
        /// <param name="pos">The location in map space of the ambient zone</param>
        /// <returns></returns>
        internal AmbientZone SetAmbientZone(Point pos)
        {
            var id = this.RenumberAmbientZones();
            var az = new AmbientZone(id, pos);
            this.AmbientZones.Add(az);
            return az;
        }

        /// <summary>
        /// Deletes an ambient zone and reorders the ids
        /// </summary>
        /// <param name="ambientZone"></param>
        internal void DeleteAmbientZones(AmbientZone ambientZone)
        {
            this.ambientZones.Remove(ambientZone);
            this.RenumberAmbientZones();
        }

        /// <summary>
        /// Renumbers the ambient zone IDs and returns the next available ID
        /// </summary>
        /// <returns></returns>
        private int RenumberAmbientZones() 
        {
            var id = 0;
            foreach (var z in this.AmbientZones)
            {
                z.Id = id;
                id++;
            }

            return id;
        }

        /// <summary>
        /// Updates the mission object from the internal SQM tree, only refreshed at export or load
        /// </summary>
        internal void UpdateFromSQM()
        {
            // start with mission details
            this.MissionDescription = this.TrySQMGet("Mission.Intel.overviewText", this.MissionDescription);
            this.MissionName = this.TrySQMGet("Mission.Intel.briefingName", this.MissionName);
            
            // get the objetive markers
            var markers = this.sqm.GetClasses("Mission.Markers", o => (
                (ParserClass)o).ContainsToken(
                    v => v.Value.ToString().StartsWith(this.ObjectiveMarkerPrefix)
                )
            );

            foreach (var marker in markers)
            {
                // find the marker ID and type
                var token = marker.GetToken("name");
                var meta = token.Value.ToString().Replace(this.ObjectiveMarkerPrefix + "_", "").Split('_');

                if (meta.Count() == 2)
                {
                    var id = int.Parse(meta[1]);
                    var pos = (ParserArray)(marker.GetToken("position"));
                    var x = (int)(double.Parse(pos.Items[0].ToString()));
                    var y = (int)(double.Parse(pos.Items[2].ToString()));

                    if (meta[0] == "obj")
                    {
                        Objective mkr = null;
                        try
                        {
                            mkr = this.Objectives.Where(o => o.Id == id).First();
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (!ex.Message.Contains("Sequence contains no elements")) throw ex;
                        }

                        if (mkr != null)
                        {
                            mkr.X = x;
                            mkr.Y = y;
                        }
                    }
                    else if (meta[0] == "amb")
                    {
                        AmbientZone mkr = null;
                        try
                        {
                            mkr = this.AmbientZones.Where(o => o.Id == id).First();
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (!ex.Message.Contains("Sequence contains no elements")) throw ex;
                        }

                        if (mkr != null)
                        {
                            mkr.X = x;
                            mkr.Y = y;
                        }
                    }
                }
            }

            // get the respawn marker
            var respawnMarkerName = "respawn_" + this.FriendlySide.ToLower();

            var respawnMarker = this.sqm.GetClasses("Mission.Markers", o => (
                (ParserClass)o).ContainsToken(
                    v => v.Value.ToString() == respawnMarkerName
                )
            );

            if (respawnMarker.Count > 0)
            {
                var pos = (ParserArray)(respawnMarker[0].GetToken("position"));
                var x = (int)(double.Parse(pos.Items[0].ToString()));
                var y = (int)(double.Parse(pos.Items[2].ToString()));
                this.RespawnX = x;
                this.RespawnY = y;
            }

        }

        /// <summary>
        /// Updates the internal SQM tree from the mission data
        /// </summary>
        internal void UpdateSQM()
        {
            // update the mission metadata
            this.sqm.Inject("Mission.Intel", new ParserObject("overviewText") { Value = this.MissionDescription });
            this.sqm.Inject("Mission.Intel", new ParserObject("briefingName") { Value = this.MissionName });

            // remove all framework markers and the main respawn
            this.sqm.RemoveChildren("Mission.Markers", o => o.Value.ToString().StartsWith(this.ObjectiveMarkerPrefix));
            this.sqm.RemoveChildren("Mission.Markers", o => o.Value.ToString().ToLower() == "respawn_" + this.FriendlySide.ToLower());
            this.sqm.Inject("Mission.Markers", new ParserObject("items") { Value = 0 });
            
            // add objective markers
            foreach (var o in this.objectives)
            {
                this.sqm.Inject("Mission.Markers", TemplateFactory.Marker(
                    o.X, o.Y, 
                    this.ObjectiveMarkerPrefix + "_obj_" + o.Id.ToString(),
                    "ColorOrange", "OBJ_" + o.Id.ToString()
                ));

                // add reward markers
                if (o.Ammo)
                {
                    this.sqm.Inject("Mission.Markers", TemplateFactory.Marker(
                        o.X + 1, o.Y + 1,
                        this.ObjectiveMarkerPrefix + "_" + o.AmmoMarker,
                        "ColorWest", "AMMO"
                    ));
                }

                if (o.Special)
                {
                    this.sqm.Inject("Mission.Markers", TemplateFactory.Marker(
                        o.X - 1, o.Y - 1,
                        this.ObjectiveMarkerPrefix + "_" + o.SpecialMarker,
                        "ColorWest", "SPECIAL"
                    ));
                }

            }

            // add the ambient markers
            foreach (var a in this.ambientZones)
            {
                this.sqm.Inject("Mission.Markers", TemplateFactory.Marker(
                    a.X, a.Y,
                    this.ObjectiveMarkerPrefix + "_amb_" + a.Id.ToString(),
                    "ColorGreen", "AMB_" + a.Id.ToString()
                ));
            }

            // add the respawn marker
            if (this.RespawnX != 0 || this.RespawnY != 0)
            {
                this.sqm.Inject("Mission.Markers", TemplateFactory.Marker(
                    this.RespawnX, this.RespawnY,
                    "respawn_" + this.FriendlySide.ToLower(),
                    "ColorGreen", "respawn_" + this.FriendlySide.ToLower()
                ));
            }

            // renumber all the marker items
            this.sqm.GetClass("Mission.Markers").Renumber();

            // remove all framework triggers
            this.sqm.RemoveChildren("Mission.Sensors", o => o.Value.ToString().StartsWith("fw_trig_obj"));
            this.sqm.Inject("Mission.Sensors", new ParserObject("items") { Value = 0 });

            // add all the objective triggers
            foreach (var obj in this.objectives)
            {
                if (obj.EndTrigger != EndTriggerTypes.None)
                {
                    var trigger = TemplateFactory.CompleteObjectiveTrigger(obj.Id, obj.EndTrigger);
                    this.sqm.Inject("Mission.Sensors", trigger);
                }
            }

            // add the end mission trigger
            if (this.EndTrigger != EndTriggerTypes.None)
            {
                var trigger = TemplateFactory.AllObjectivesTrigger(this.EndTrigger.ToString());
                this.sqm.Inject("Mission.Sensors", trigger);
            }

            // add the key objective trigger
            if (this.KeyObjectiveVictoryTrigger != EndTriggerTypes.None)
            {
                var trigger = TemplateFactory.KeyObjectivesTrigger(this.KeyObjectiveVictoryTrigger.ToString(), this.objectives.Where(o => o.IsKeyObjective));
                this.sqm.Inject("Mission.Sensors", trigger);
            }

            // renumber the triggers
            this.sqm.GetClass("Mission.Sensors").Renumber();
        }

        /// <summary>
        /// Gets a value from the SQM or returns a default if no value found
        /// </summary>
        /// <typeparam name="T">The type of object to get</typeparam>
        /// <param name="path">The path in the SQM document to get</param>
        /// <param name="defaultValue">The value to return if the path is not found</param>
        /// <returns></returns>
        private T TrySQMGet<T>(string path, T defaultValue)
        {
            try
            {
                return (T)(this.sqm.GetToken(path).Value);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets a list of objectives in this mission
        /// </summary>
        [Browsable(false)]
        public List<Objective> Objectives 
        {
            get
            {
                return this.objectives;
            }
        }

        /// <summary>
        /// Gets a list of ambient zones in this mission
        /// </summary>
        [Browsable(false)]
        public List<AmbientZone> AmbientZones
        {
            get
            {
                return this.ambientZones;
            }
        }

        /// <summary>
        /// Gets a list of the scripts that are included in the mission
        /// </summary>
        [Category("Details")]
        [DisplayName("Included Scripts")]
        [Description("A list of scripts which will be included in the mission output folder")]
        [Browsable(false)]
        public List<string> IncludedScripts
        {
            get
            {
                return this.includedScripts;
            }
        }

        /// <summary>
        /// The briefing that is written to briefing.sqf
        /// </summary>
        [Browsable(false)]
        public Briefing MissionBriefing { get; set; }

        /// <summary>
        /// Gets a list of the scripts that are available to be used
        /// </summary>
        internal List<ScriptInclude> AvailableScripts
        {
            get
            {
                return this.availableScripts;
            }
        }

        /// <summary>
        /// A name to describe the mission (for the description.ext)
        /// </summary>
        [Category("Details")]
        [DisplayName("Mission Name")]
        [Description("The mission name used in the description.ext")]
        public string MissionName { get; set; }

        /// <summary>
        /// A description of the mission (for the description.ext)
        /// </summary>
        [Category("Details")]
        [DisplayName("Mission Description")]
        [Description("The mission description used in the description.ext")]
        public string MissionDescription { get; set; }

        /// <summary>
        /// The author of the mission (for the description.ext)
        /// </summary>
        [Category("Details")]
        [DisplayName("Author")]
        [Description("The author of the mission")]
        public string MissionAuthor { get; set; }

        /// <summary>
        /// The x coordinate of the initial spawn position
        /// </summary>
        [Category("Respawn")]
        [DisplayName("X Coordinate")]
        [Description("The x coordinate of the initial spawn position")]
        public int RespawnX { get; set; }

        /// <summary>
        /// The y coordinate of the initial spawn position
        /// </summary>
        [Category("Respawn")]
        [DisplayName("Y Coordinate")]
        [Description("The y coordinate of the initial spawn position")]
        public int RespawnY { get; set; }

        [Category("Details")]
        [DisplayName("Enemy Side")]
        [Description("The side that enemy spawns should be added to")]
        [ItemsSource(typeof(SideItemSource))]
        public string EnemySide { get; set; }

        [Category("Details")]
        [DisplayName("Random Objective Order")]
        [Description("If set to true, then objectives will be unlocked one by one in a random order. This ignores any prerequisites placed in the editor and gives a different order each time the mission is loaded")]
        public bool RandomObjectiveOrder { get; set; }

        [Category("Victory")]
        [DisplayName("Trigger on all completed")]
        [Description("The type of trigger that should be created when all objectives are completed")]
        public EndTriggerTypes EndTrigger { get; set; }

        [Category("Victory")]
        [DisplayName("Trigger on Key Objectives")]
        [Description("The type of trigger that should be created when all objectives KEY are completed")]
        public EndTriggerTypes KeyObjectiveVictoryTrigger { get; set; }

        [Category("Details")]
        [DisplayName("Friendly Side")]
        [Description("The side that players will be playing as")]
        [ItemsSource(typeof(SideItemSource))]
        public string FriendlySide { get; set; }

        [Category("Details")]
        [DisplayName("Debug Console")]
        [Description("Who should be able to see the debug console in multiplayer?")]
        [ItemsSource(typeof(DebugConsoleItemSource))]
        public int DebugConsole { get; set; }

        [Category("Scripting")]
        [DisplayName("Custom init.sqf code")]
        [Description("Custom init.sqf that is placed at the end of the file")]
        public string InitSqfCode { get; set; }

        /// <summary>
        /// Gets or sets the base SQM model that underlies this mission
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public ParserClass SQM
        {
            get
            {
                return this.sqm;
            }
            set
            {
                this.sqm = value;
            }
        }
    }
}
