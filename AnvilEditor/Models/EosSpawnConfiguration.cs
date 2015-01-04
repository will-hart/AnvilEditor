using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilEditor.Models
{
    public class EosSpawnConfiguration
    {
        public List<string> InfantryPool { get; set; }
        public List<string> ArmourPool { get; set; }
        public List<string> MotorisedPool { get; set; }
        public List<string> AttackHeliPool { get; set; }
        public List<string> HeliPool { get; set; }
        public List<string> UavPool { get; set; }
        public List<string> StaticPool { get; set; }
        public List<string> ShipPool { get; set; }
        public List<string> DiverPool { get; set; }
        public List<string> CrewPool { get; set; }
        public List<string> HeliCrewPool { get; set; }

        public EosSpawnConfiguration()
        {
            this.InfantryPool = new List<string>();
            this.ArmourPool = new List<string>();
            this.MotorisedPool = new List<string>();
            this.AttackHeliPool = new List<string>();
            this.HeliPool = new List<string>();
            this.UavPool = new List<string>();
            this.StaticPool = new List<string>();
            this.ShipPool = new List<string>();
            this.DiverPool = new List<string>();
            this.CrewPool = new List<string>();
            this.HeliCrewPool = new List<string>();
        }

        /// <summary>
        /// Clones an eos configuration
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        public EosSpawnConfiguration Clone()
        {
            var eos = new EosSpawnConfiguration();

            eos.InfantryPool = new List<string>(this.InfantryPool);
            eos.ArmourPool = new List<string>(this.ArmourPool);
            eos.MotorisedPool = new List<string>(this.MotorisedPool);
            eos.AttackHeliPool = new List<string>(this.AttackHeliPool);
            eos.HeliPool = new List<string>(this.HeliPool);
            eos.UavPool = new List<string>(this.UavPool);
            eos.StaticPool = new List<string>(this.StaticPool);
            eos.ShipPool = new List<string>(this.ShipPool);
            eos.DiverPool = new List<string>(this.DiverPool);
            eos.CrewPool = new List<string>(this.CrewPool);
            eos.HeliCrewPool = new List<string>(this.HeliCrewPool);

            return eos;
        }

        /// <summary>
        /// Converts the spawn configuration to an SQM array suitable for exporting to the mission_definition.sqm file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sqm = "[" + Environment.NewLine;

            sqm += string.Format("\t[{0}],", string.Join(",", this.InfantryPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.ArmourPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.MotorisedPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.AttackHeliPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.HeliPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.UavPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.StaticPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.ShipPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.DiverPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}],", string.Join(",", this.CrewPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;
            sqm += string.Format("\t[{0}]", string.Join(",", this.HeliCrewPool.Select(o => "\"" + o + "\""))) + Environment.NewLine;

            sqm += "]";
            return sqm;
        }
    }
}
