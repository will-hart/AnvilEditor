using Newtonsoft.Json;
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

namespace AnvilEditor.Models
{
    /// <summary>
    /// Holds objective data that can be written to file
    /// </summary>
    public class Objective : ObjectiveBase
    {
        /// <summary>
        /// A list of prereq ID numbers to unlock this objective
        /// </summary>
        private readonly List<int> prereqs = new List<int>();

        /// <summary>
        /// Default constructor, creates a new objective with the given id
        /// </summary>
        /// <param name="id">The ID number to use to refer to this objective</param>
        public Objective(int id, Point location)
            : base(id, location)
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
            this.EndTrigger = EndTriggerTypes.None;
        }

        /// <summary>
        /// Adds a prerequisite objective if it doesn't already exist
        /// </summary>
        /// <param name="id">The id of the prerequisite objective</param>
        internal void AddPrerequisite(int id)
        {
            if (!this.prereqs.Contains(id))
            {
                this.prereqs.Add(id);
            }
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
        /// Gets or sets a value used to describe this objective
        /// </summary>
        [Category("Details")]
        [Description("The description that will appear in the in-game task")]
        public string Description { get; set; }

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
        [DisplayName("Detailed Description")]
        [Description("Text that will be added to the in game objective showing the rewards that will be unlocked upon capturing this objective")]
        public string RewardDescription { get; set; }

        [Category("Details")]
        [DisplayName("Objective Type")]
        [Description("The type of mission that will be generated on the marker")]
        [ItemsSource(typeof(MissionTypeItemSource))]
        public int ObjectiveType { get; set; }

        [Category("Details")]
        [DisplayName("Trigger on complete")]
        [Description("The type of trigger that should be created when this objective is completed")]
        public EndTriggerTypes EndTrigger { get; set; }

        [Category("Details")]
        [DisplayName("Is a Key Objective")]
        [Description("If 'Key Objective Victory' is enabled, this must be captured before victory can occur")]
        public bool IsKeyObjective { get; set; }

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
    }
}
