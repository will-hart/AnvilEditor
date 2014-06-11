using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;

namespace AnvilParser
{
    public class MissionBase : ParserClass
    {
        /// <summary>
        /// Handles random seeds
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Creates a new mission base, completely unpopulated
        /// </summary>
        /// <param name="name"></param>
        public MissionBase(string name)
            : this(name, false)
        {
        }

        /// <summary>
        /// Creates a new mission base, with the `populate` flag determining if the class should be prepopulated with 
        /// default mission classes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="populate"></param>
        public MissionBase(string name, bool populate) : base(name)
        {
            if (populate)
            {
                if (name == "root")
                {
                    this.Add("version", 12);
                    this.Add(new MissionBase("Mission", true));
                    this.Add(new MissionBase("Intro", true));
                    this.Add(new MissionBase("OutroLoose", true));
                    this.Add(new MissionBase("OutroWin", true));
                }
                else
                {
                    this.Add("Addons", new List<object> { "a3_map_altis" });
                    this.Add("addOnsAuto", new List<object> { "a3_map_altis" });
                    this.Add("randomSeed", Rand.Next());
                    this.Add(new IntelBase());
                }
            }
        }

        public MissionBase(string name, bool populate, IEnumerable<IParserToken> tokens, IEnumerable<ParserClass> objects)
            : this(name, populate)
        {
            this.Tokens = tokens.ToList();
            this.Objects = objects.ToList();
        }
    }
}
