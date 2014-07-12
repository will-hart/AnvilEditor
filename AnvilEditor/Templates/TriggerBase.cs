using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser;
using AnvilParser.Tokens;

namespace AnvilEditor.Templates
{
    public class TriggerBase : ParserClass
    {
        public TriggerBase()
            : base("Item0")
        {
            this.Add(new ParserArray("position") { Items = new List<object> { 0.0, 0.0, 0.0 } });
            this.Add("a", 0);
            this.Add("b", 0);
            this.Add("interruptable", 1);
            this.Add("age", "UNKNOWN");

            this.Add(new ParserClass("Effects"));
        }
    }
}
