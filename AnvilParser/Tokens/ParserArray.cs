using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilParser.Tokens
{
    public class ParserArray : IParserToken
    {
        /// <summary>
        /// Hold the items
        /// </summary>
        private List<object> items;

        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        public string ToSQM()
        {
            return this.Name + "[] = {" + string.Join(", ", this.items.ToString()) + "};";
        }

        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a list of items attached to this object
        /// </summary>
        public List<object> Items
        {
            get
            {
                return this.items;
            }

            set
            {
                this.items = value;
            }
        }
    }
}
