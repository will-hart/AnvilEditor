using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilEditor.Models
{
    public class Briefing
    {
        public Briefing()
        {
            this.BriefingSections = new List<string>();
            this.BriefingParts = new Dictionary<string, string>();
        }

        /// <summary>
        /// Provides a string output of the briefing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var output = @"waitUntil { !isNil {player} };
waitUntil { player == player };

";
            var tpl = "player createDiaryRecord [\"Diary\", [\"{0}\", \"{1}\"]];";
            var sectOutput = Enumerable.Reverse(this.BriefingSections);
            foreach (var s in sectOutput)
            {
                output += string.Format(tpl, s, this.BriefingParts.ContainsKey(s) ? this.BriefingParts[s].Replace(Environment.NewLine, "<br/>") : "") + Environment.NewLine;
            }

            return output;
        }

        /// <summary>
        /// Deletes a section from the briefing
        /// </summary>
        /// <param name="p">The name of the section to delete</param>
        internal void Delete(string p)
        {
            this.BriefingSections.Remove(p);
            this.BriefingParts.Remove(p);
        }

        /// <summary>
        /// Adds or updates a section to the briefing
        /// </summary>
        /// <param name="p"></param>
        internal void Set(string key, string value="")
        {
            if (key == "")
            {
                return;
            }

            if (!this.BriefingSections.Contains(key))
            {
                this.BriefingSections.Add(key);
            }

            if (!this.BriefingParts.ContainsKey(key))
            {
                this.BriefingParts.Add(key, value);
            }
            else
            {
                this.BriefingParts[key] = value;
            }
        }

        /// <summary>
        /// Gets a briefing section or returns an empty string if the key is unknown
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal string Get(string key)
        {
            if (!this.BriefingParts.ContainsKey(key) || key == "")
            {
                return "";
            }
            return this.BriefingParts[key];
        }

        /// <summary>
        /// The briefing sections that are displayed in the mission editor
        /// </summary>
        public Dictionary<string, string> BriefingParts { get; set; }

        /// <summary>
        /// The ordered sections of the briefing. This is the main source of briefing sections
        /// </summary>
        public List<string> BriefingSections { get; set; }

    }
}
