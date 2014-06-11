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
                return this.Name + " = \"" + this.Value + "\";";
            }
            return this.Name + " = " + this.Value.ToString() + ";";
        }

        /// <summary>
        /// Returns a string representation of the value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.GetType() == typeof(string) ? "\"" + this.Value + "\"" : this.Value.ToString();
        }

        /// <summary>
        /// Injects a value, into this token
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        public void Inject(string path, IParserToken token)
        {
            this.Value = token.Value;
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
