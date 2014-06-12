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
        /// Default constructor for a parser object
        /// </summary>
        public ParserArray(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        public string ToSQM(int level = 0)
        {
            var first = new String(' ', 4 * level);
            var others = first + "    ";

            return first + this.Name + "[] = {" + Environment.NewLine + others +
                string.Join(
                    ", " + Environment.NewLine + others, 
                    this.items.Select(o => o.GetType() == typeof(string) ? "\"" + o + "\"" : o.ToString())
                ) + 
                Environment.NewLine + first + "};";
        }

        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gives a string representation of the value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[ " + string.Join(", ", this.items.Select(o => 
                  o.GetType() == typeof(string) ? "\"" + o + "\"" : o.ToString()
            )) + "]";
        }

        /// <summary>
        /// Injects a series of values into this array
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        public void Inject(string path, IParserToken token)
        {
            this.Items.Add(token.Value);
        }

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

        public object Value
        {
            get
            {
                return (object)this.items;
            }
        }
    }
}
