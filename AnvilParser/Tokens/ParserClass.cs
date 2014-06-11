using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;

namespace AnvilParser
{
    public class ParserClass : IParserToken
    {
        /// <summary>
        /// A list of objects contained by this class
        /// </summary>
        private readonly Dictionary<string, ParserClass> objects = new Dictionary<string, ParserClass>();

        /// <summary>
        /// A list of tokens contained within this object
        /// </summary>
        private readonly Dictionary<string, IParserToken> tokens = new Dictionary<string, IParserToken>();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        public ParserClass(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor which allows setting of initial tokens and objects
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tokens"></param>
        /// <param name="objects"></param>
        public ParserClass(string name, List<IParserToken> tokens, List<ParserClass> objects)
        {
            this.Name = name;
            this.Tokens = tokens;
            this.Objects = objects;
        }

        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        public string ToSQM()
        {
            var op = string.Empty;

            if (this.Name != "root")
            {
                op = @"class " + this.Name + Environment.NewLine + "{" + Environment.NewLine;
            }

            op += string.Join(Environment.NewLine, this.tokens.Select(o => o.Value.ToSQM()));
            if (this.tokens.Count > 0) op += Environment.NewLine;

            op += string.Join(Environment.NewLine, this.objects.Select(o => o.Value.ToSQM())) + Environment.NewLine;
            if (this.objects.Count > 0) op += Environment.NewLine;

            if (this.Name != "root")
            {
                op += Environment.NewLine + "};";
            }

            return op;
        }

        /// <summary>
        /// Adds a ParserArray to the 
        /// </summary>
        /// <param name="name">The token name (left of the equals sign)</param>
        /// <param name="value">The token value - should implement "ToString()"</param>
        /// <returns></returns>
        public ParserArray Add(string name, List<object> value)
        {
            var obj = new ParserArray();
            obj.Name = name;
            obj.Items = value;
         
            if (!this.tokens.Keys.Contains(name))
            {
                this.tokens.Add(name, obj);
            }
            else
            {
                this.tokens[name] = obj;
            }

            return obj;
        }

        /// <summary>
        /// Adds an object to the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ParserObject AddObject(string name, object value) {
            
            var obj = new ParserObject();
            obj.Name = name;
            obj.Value = value;
         
            if (!this.tokens.Keys.Contains(name))
            {
                this.tokens.Add(name, obj);
            }
            else
            {
                this.tokens[name] = obj;
            }

            return obj;
        }

        /// <summary>
        /// Overloaded add object method for an integer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParserObject Add(string name, int value)
        {
            return this.AddObject(name, value);
        }

        /// <summary>
        /// Overloaded add object method for a double
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParserObject Add(string name, double value)
        {
            return this.AddObject(name, value);
        }

        /// <summary>
        /// Overloaded add object method for a string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParserObject Add(string name, string value)
        {
            return this.AddObject(name, value);
        }
        
        /// <summary>
        /// Adds a child parser class and returns it
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public ParserClass Add(ParserClass cls)
        {
            if (!this.objects.ContainsKey(cls.Name))
            {
                this.objects.Add(cls.Name, cls);
            }
            else
            {
                this.objects[cls.Name] = cls;
            }

            return cls;
        }

        /// <summary>
        /// Injects a value in to the mission using a dot-separated path
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Inject(string name, object value)
        {
            var addr = name.Split(new char[] { '.' }, 2);

            if (addr.Count() == 1)
            {
                if (value.GetType() == typeof(ParserClass)) 
                {
                    // handle classes
                    this.Add((ParserClass)value);
                }
                else 
                {
                    // handle tokens
                    this.AddObject(name, value);
                }
            }
            else
            {
                if (this.objects.ContainsKey(addr[0]))
                {
                    this.objects[addr[0]].Inject(addr[1], value);
                }
                else
                {
                    throw new ArgumentException("Unknown object path for " + this.Name + ": " + name);
                }
            }
        }

        /// <summary>
        /// Provide a string representation of this class (in this case the SQM)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToSQM();
        }

        /// <summary>
        /// Gets or sets the tokens assigned to this class
        /// </summary>
        public List<IParserToken> Tokens
        {
            get
            {
                return this.tokens.Values.ToList();
            }
            set
            {
                this.tokens.Clear();
                foreach (var t in value)
                {
                    this.tokens.Add(t.Name, t);
                }
            }
        }

        /// <summary>
        /// Gets or sets the class objects attached to this object
        /// </summary>
        public List<ParserClass> Objects
        {
            get
            {
                return this.objects.Values.ToList();
            }
            set 
            {
                this.objects.Clear();
                foreach (var o in value)
                {
                    this.objects.Add(o.Name, o);
                }
            }
        }

        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; }
    }
}
