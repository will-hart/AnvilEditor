using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TacticalAdvanceMissionBuilder
{
    public class Mission
    {
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
        [DisplayName("Marker Prefix")]
        [Category("Details")]
        [Description("The prefix to add to marker names")]
        public string ObjectiveMarkerPrefix { get; set; }

        /// <summary>
        /// The Item number in the mission SQM file to start counting objective markers from
        /// </summary>
        [Category("Details")]
        [DisplayName("Additional Markers")]
        [Description("The number of markers that come after this (added in the editor). WARNING - take care when adding markers in the editor then adding new objectives and regenerating, as some duplicate marker names may appear.")]
        public int ObjectiveMarkerOffset { get; set; }

        public Mission()
        {
            this.ObjectiveMarkerPrefix = "fw_obj";
            this.ObjectiveMarkerOffset = 0;
            this.MissionName = "Anvil Mission";
            this.MissionDescription = "A mission made with Anvils";

            // load in the supported scripts
            var path = System.IO.Path.Combine( 
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "supported_scripts.json"
            );
            using (var sr = new StreamReader(path))
            {
                var json = sr.ReadToEnd();
                this.availableScripts = JsonConvert.DeserializeObject<List<ScriptInclude>>(json);
            }
        }

        /// <summary>
        /// Get an objective by ID
        /// </summary>
        /// <param name="id">The id to return an objective for</param>
        /// <returns>An objective with the given Id or null</returns>
        public Objective GetObjective(int id)
        {
            return this.objectives.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Deletes the objective with the given id
        /// </summary>
        /// <param name="id">The id of the objective to delete</param>
        public void DeleteObjective(int id)
        {
            var obj = this.GetObjective(id);
            this.DeleteObjective(obj);
        }

        /// <summary>
        /// Deletes the objective object given
        /// </summary>
        /// <param name="obj">The objective object to delete</param>
        public void DeleteObjective(Objective obj) 
        {
            this.objectives.Remove(obj);

            foreach (var o in this.objectives)
            {
                o.Prerequisites.Remove(obj.Id);
            }

            this.availableIds.Add(obj.Id);
        }

        /// <summary>
        /// Clears a mission back to a new state
        /// </summary>
        public Mission ClearMission()
        {
            return new Mission();
        }

        /// <summary>
        /// Adds a new objective to the mission
        /// </summary>
        /// <param name="location">The location of the objective</param>
        /// <returns>The objective that was just created</returns>
        public Objective AddObjective(Point location)
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

    }
}
