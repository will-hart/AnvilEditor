using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilParser
{
    public class ParserContainer
    {
        private readonly ParserClass baseClass = new ParserClass();

        /// <summary>
        /// Creates a new default instance of a parser class
        /// </summary>
        /// <param name="name"></param>
        public ParserContainer(string name)
        {
            this.baseClass.Name = name;
        }
    }
}
