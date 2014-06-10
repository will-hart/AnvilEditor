using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilParser.Tokens
{
    public class ParserObject : IParserToken
    {
        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        public string ToSQM()
        {
            if (this.Value.GetType() == typeof(string))
            {
                return this.Name + " = \"" + string.Join(", ", this.Value) + "\";";
            }
            return this.Name + " = " + this.Value.ToString() + ";";
        }

        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value of the element
        /// </summary>
        public object Value { get; set; }
    }
}
