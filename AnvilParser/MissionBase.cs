using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;

namespace AnvilParser
{
    class MissionBase : ParserClass
    {
        public MissionBase(string name)
            : base(name)
        {
            this.Add("Addons", new List<object> { "a3_map_altis" });
            this.Add("addOnsAuto", new List<object> { "a3_map_altis" });

            this.Add(new ParserClass("intel"));

            this.Add("randomSeed", (new Random()).Next());
        }
    }
}
