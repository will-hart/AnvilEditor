namespace AnvilEditor.Templates
{
    using System.Collections.Generic;

    using AnvilParser.Tokens;

    /// <summary>
    /// A template which can be loaded from JSON and injects 
    /// </summary>
    public class InjectedTemplate
    {
        /// <summary>
        /// A list of components that are injected by this template
        /// </summary>
        private readonly Dictionary<string, IParserToken> components = new Dictionary<string, IParserToken>();

        /// <summary>
        /// Applies the template to the mission base
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="arguments"></param>
        internal void Apply(MissionBase mission, Dictionary<string, object> arguments)
        {
            return;
        }

        /// <summary>
        /// Gets a dictionary of Template Tokens for injection into the mission SQM
        /// </summary>
        public Dictionary<string, IParserToken> Components
        {
            get
            {
                return this.components;
            }
        }

        /// <summary>
        /// The name of the template in the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the template in the UI
        /// </summary>
        public string Description { get; set; }
    }
}
