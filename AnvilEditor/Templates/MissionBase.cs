namespace AnvilEditor.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AnvilParser;
    using AnvilParser.Tokens;

    public class MissionBase : ParserClass
    {
        /// <summary>
        /// Handles random seeds
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Creates a new mission base, with the `populate` flag determining if the class should be prepopulated with 
        /// default mission classes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="populate"></param>
        public MissionBase(string name) : base(name)
        {
            if (name == "root")
            {
                this.Add("version", 12);
                this.Add(new MissionBase("Mission"));
                this.Add(new MissionBase("Intro"));
                this.Add(new MissionBase("OutroLoose"));
                this.Add(new MissionBase("OutroWin"));

                this.Inject("Mission.Groups", new ParserObject("items") { Value = 1 });
                this.Inject("Mission.Groups", new ServerBase());
                this.Inject("Mission.Intel", new ParserObject("resistanceWest") { Value = 0 });
            }
            else
            {
                this.Add("Addons", new List<object> { "a3_map_altis" });
                this.Add("addOnsAuto", new List<object> { "a3_map_altis" });
                this.Add("randomSeed", Rand.Next());
                this.Add(new IntelBase());

                if (name == "Missions")
                {
                    this.Add(new ParserClass("Groups"));
                    this.Add(new ParserClass("Vehicles"));
                    this.Add(new ParserClass("Sensors"));
                }
            }
        }

        public MissionBase(string name, IEnumerable<IParserToken> tokens, IEnumerable<ParserClass> objects)
            : this(name)
        {
            this.Tokens = tokens.ToList();
            this.Objects = objects.ToList();
        }
    }
}
