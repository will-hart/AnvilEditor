﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser;
using AnvilParser.Tokens;
using System.IO;
using Newtonsoft.Json;

namespace AnvilEditor.Templates
{
    internal static class TemplateFactory
    {
        /// <summary>
        /// Custom user templates loaded from JSON
        /// </summary>
        private static readonly Dictionary<string, InjectedTemplate> Templates = new Dictionary<string, InjectedTemplate>();

        /// <summary>
        /// Gets a template of the given name from the template dictionary
        /// </summary>
        /// <param name="name"></param>
        internal static InjectedTemplate GetTemplate(string name, MissionBase mission, List<object> arguments) {
            if (!Templates.ContainsKey(name)) throw new ArgumentException("Unknown template name - " + name);
            return Templates[name];
        }

		/// <summary>
		/// Creates a marker as an SQM Parser object and returns it
		/// </summary>
		/// <param name="x">The x position of the marker</param>
		/// <param name="y">The y position of the marker</param>
		/// <param name="name">The name to use for the marker (should already include fw_ prefix)</param>
        /// <param name="color">(OPTIONAL, DEFAULT="colorOrange") The colour of the marker</param>
		/// <param name="text">(OPTIONAL, DEFAULT="") The text to display for the marker</param>
		/// <returns></returns>
        internal static ParserClass Marker(int x, int y, string name, string color="ColorOrange", string text="")
        {
			// create a new marker with a temporary name
            var mkr = new ParserClass("Item000_" + name);
            mkr.Add(new ParserArray("position") { Items = new List<object> { x, 0.0m, y } });
            mkr.Add(new ParserObject("name") { Value = name });
            mkr.Add(new ParserObject("type") { Value = "Empty" });
            mkr.Add(new ParserObject("colorName") { Value = color });
            mkr.Add(new ParserObject("text") { Value = text });
			return mkr;
        }

        /// <summary>
        /// Returns a populated mission base for use with a new Anvil generated mission
        /// </summary>
        /// <returns></returns>
        internal static ParserClass Mission()
        {
            return new MissionBase("root");
        }

        /// <summary>
        /// Loads all Injected templates in a given folder into memory
        /// </summary>
        /// <param name="directoryPath"></param>
        internal static void LoadAllTemplates(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;
            var dirInfo = new DirectoryInfo(directoryPath);
            var files = dirInfo.GetFiles();

            // get all the JSON files
            foreach (var f in files.Where(o => o.Name.EndsWith("json")))
            {
                using (var sr = new StreamReader(f.FullName))
                {
                    var json = sr.ReadToEnd();
                    var tpl = JsonConvert.DeserializeObject<InjectedTemplate>(json);
                    Templates.Add(tpl.Name, tpl);
                }
            }
        }
    }
}
