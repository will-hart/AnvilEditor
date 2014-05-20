using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Holds objective data that can be written to file
    /// </summary>
    public class Objective
    {
        /// <summary>
        /// The boundaries of the map for converting screen to map coordinates
        /// </summary>
        private static double MapXMin = 2000;
        private static double MapXMax = 30000;
        private static double MapYMin = 5000;
        private static double MapYMax = 26000;
        private static double ScreenXMax = 800;
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
                return MapXMin + (this.screenX / ScreenXMax) * (MapXMax - MapXMin);
            }
            set
            {
                this.screenX = ScreenXMax * ((value - MapXMin) / (MapXMax - MapXMin));
            }
        }

        [Category("Location")]
        [Description("The Y coordinate for this objective")]
        public double Y
        {
            get
            {
                return MapYMin + (this.screenY / ScreenYMax) * (MapYMax - MapYMin);
            }
            set
            {
                this.screenY = ScreenYMax * ((value - MapYMin) / (MapYMax - MapYMin));
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
