using System;
using System.Collections.Generic;
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
        public string ObjectiveMarkerPrefix { get; set; }

        /// <summary>
        /// The Item number in the mission SQM file to start counting objective markers from
        /// </summary>
        public int ObjectiveMarkerOffset { get; set; }

        public Mission()
        {
            this.ObjectiveMarkerPrefix = "fw_obj";
            this.ObjectiveMarkerOffset = 11;
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
        /// Gets a list of objectives in this mission
        /// </summary>
        public List<Objective> Objectives 
        {
            get
            {
                return this.objectives;
            }
        }
    }
}
