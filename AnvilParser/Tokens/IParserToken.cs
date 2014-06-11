using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilParser.Tokens
{
    public interface IParserToken
    {
        /// <summary>
        /// The name of the element
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        string ToSQM();

        /// <summary>
        /// Returns a string representation of the value
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
