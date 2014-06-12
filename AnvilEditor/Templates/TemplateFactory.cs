using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser;
using AnvilParser.Tokens;

namespace AnvilEditor.Templates
{
    internal static class TemplateFactory
    {
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
    }
}
