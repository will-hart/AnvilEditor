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
         
            if (!this.tokens.Keys.Contains(name))
            {
                var obj = new ParserArray(name);
                obj.Items = value;
                this.tokens.Add(name, obj);
                return obj;
            }
            else
            {
                if (this.tokens[name].GetType() == typeof(ParserArray)) {
                    var token = ((ParserArray)this.tokens[name]);
                    foreach (var obj in value)
                    {
                        token.Items.Add(obj);
                    }

                    return token;
                }
                else 
                {
                    throw new ArgumentException("Invalid Array 'add' - the selected SQM token is not an array");
                }
            }
        }

        /// <summary>
        /// Adds an object to the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ParserObject AddObject(string name, object value) {

            var obj = new ParserObject(name);
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
        /// Adds a token to the object
        /// </summary>
        /// <param name="tok"></param>
        public void Add(IParserToken tok)
        {
            if (tok.GetType() == typeof(ParserClass))
            {
                this.Add((ParserClass)tok);
            }
            else
            {
                this.tokens.Add(tok.Name, tok);
            }
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
        public void Inject(string name, IParserToken token)
        {            
            var addr = name.Split(new char[] { '.' }, 2);

            if (name == "") {
                if (!this.tokens.ContainsKey(token.Name) && !this.objects.ContainsKey(token.Name)) 
                {
                    this.Add(token);
                }
                else
                {
                    IParserToken target = this.tokens[token.Name];
                    var tokenType = token.GetType();
                    var targetType = target.GetType();
                
                    // perform injection
                    if (targetType == typeof(ParserClass)) 
                    {
                        if (tokenType == typeof(ParserObject))
                        {
                            ((ParserClass)target).AddObject(token.Name, token.Value);
                        }
                        else if (tokenType == typeof(ParserArray))
                        {
                            ((ParserClass)target).Add(token.Name, ((ParserArray)token).Items);
                        }
                        else
                        {
                            ((ParserClass)target).Merge((ParserClass)token);
                        }
                        
                    }
                    else if (tokenType == typeof(ParserObject) && 
                            ( targetType == typeof(ParserObject) || targetType == typeof(ParserArray)))
                    {
                        target.Inject(addr[0], token);
                    }
                    // can also put an array into an array
                    else if (tokenType == typeof(ParserArray) && 
                        targetType == typeof(ParserArray))
                    {
                        foreach (var item in ((ParserArray)token).Items)
                        {
                            target.Inject("", new ParserObject(addr[0]) { Value = item });
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid SQM Token injection - tried to inject a " + token.GetType().ToString() +
                            " into a " + target.GetType().ToString());
                    }
                }
            } 
            else 
            {
                // dive down, creating classes as we go
                if (!this.objects.ContainsKey(addr[0])) {
                    var npc = new ParserClass(addr[0]);
                    this.Add(npc);
                }
                this.objects[addr[0]].Inject(addr.Count() == 2 ? addr[1] : "", token); 
            }
        }

        /// <summary>
        /// Removes the tokens and objects at the given path from the mission
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            var addr = name.Split(new char[] { '.' }, 2);

            if (addr.Count() == 1)
            {
                this.tokens.Remove(name);
                this.objects.Remove(name);
            }
            else
            {
                if (this.objects.ContainsKey(addr[0]))
                {
                    this.objects[addr[0]].Remove(addr[1]);
                }
                else
                {
                    throw new ArgumentException("Unknown object path for removal on " + this.Name + ": " + name);
                }
            }
        }

        /// <summary>
        /// Performs deletion of mission objects at this level where their names satisfy the predicate
        /// </summary>
        /// <param name="selector"></param>
        private void Remove(Func<string, bool> selector)
        {
            var removalPaths = new List<string>();

            foreach (var t in this.tokens.Keys)
            {
                if (selector.Invoke(t)) removalPaths.Add(t);
            }

            foreach (var o in this.objects.Keys)
            {
                if (o.StartsWith("Item"))
                {
                    // apply to nested items
                    this.objects[o].Remove(selector);
                }
                else if (selector.Invoke(o))
                { 
                    removalPaths.Add(o); 
                }
            }

            foreach (var r in removalPaths)
            {
                this.tokens.Remove(r);
                this.objects.Remove(r);
            }
        }

        /// <summary>
        /// Removes the objects and tokens at the given path who have names which satisfy the passed selector lambda
        /// </summary>
        /// <param name="path">The root path to delete items from</param>
        /// <param name="selector">The selector to apply to chose items being deleted</param>
        public void Remove(string path, Func<string, bool> selector)
        {
            var addr = path.Split(new char[] { '.' }, 2);

            if (addr.Count() == 1)
            {
                this.objects[addr[0]].Remove(selector);
            }
            else
            {
                if (this.objects.ContainsKey(addr[0]))
                {
                    this.objects[addr[0]].Remove(addr[1], selector);
                }
                else
                {
                    throw new ArgumentException("Unknown object path for removal on " + this.Name + ": " + path);
                }
            }
        }

        /// <summary>
        /// Checks whether the current ParserClass contains any tokens matching the given predicate
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool ContainsToken(Func<IParserToken, bool> selector)
        {
            return this.tokens.Values.Any(selector);
        }

        /// <summary>
        /// Performs a removal of all child tokens matching a given signature
        /// </summary>
        /// <param name="selector"></param>
        private void RemoveChildren(Func<IParserToken, bool> selector)
        {
            var oRem = this.objects.Values.Where(o => o.ContainsToken(selector)).ToList();
            var tRem = this.tokens.Values.Where(selector).ToList();

            foreach (var o in oRem)
            {
                this.objects.Remove(o.Name);
            }

            foreach (var t in tRem)
            {
                this.tokens.Remove(t.Name);
            }
        }

        /// <summary>
        /// Removes all child objects which contain a given predicate
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selector"></param>
        public void RemoveChildren(string path, Func<IParserToken, bool> selector)
        {
            var addr = path.Split(new char[] { '.' }, 2);

            if (!this.objects.ContainsKey(addr[0]))
            {
                throw new ArgumentException("Unknown object path for removal on " + this.Name + ": " + path);
            }

            if (addr.Count() == 1)
            {
                this.objects[addr[0]].RemoveChildren(selector);
            }
            else
            {
                this.objects[addr[0]].RemoveChildren(addr[1], selector);
            }
        }

        /// <summary>
        /// Gets a specific token at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IParserToken GetToken(string path)
        {
            var addr = path.Split(new char[] { '.' }, 2);
            
            if (addr.Count() == 1)
            {
                if (!this.tokens.ContainsKey(addr[0]))
                {
                    return null;
                }
                return this.tokens[addr[0]];
            }
            else
            {
                return this.objects[addr[0]].GetToken(addr[1]);
            }
        }

        /// <summary>
        /// Gets a specific class at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ParserClass GetClass(string path)
        {
            var addr = path.Split(new char[] { '.' }, 2);

            if (!this.objects.ContainsKey(addr[0]))
            {
                return null;
            }

            if (addr.Count() == 1)
            {
                return this.objects[addr[0]];
            }
            else
            {
                return this.objects[addr[0]].GetClass(addr[1]);
            }
        }

        /// <summary>
        /// Gets attached classes matching the given predicate
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public List<IParserToken> GetClasses(string path, Func<IParserToken, bool> selector)
        {
            var addr = path.Split(new char[] { '.' }, 2);

            if (!this.objects.ContainsKey(addr[0]))
            {
                throw new ArgumentException("Unknown object path for removal on " + this.Name + ": " + path);
            }

            if (addr.Count() == 1)
            {
                return this.objects[addr[0]].Objects.Where(selector).ToList();
            }
            else
            {
                return this.objects[addr[0]].GetClasses(addr[1], selector);
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
        /// Merges all of the elements of another class into this one
        /// </summary>
        /// <param name="other"></param>
        private void Merge(ParserClass other)
        {
            foreach (var t in other.Tokens)
            {
                this.AddObject(t.Name, t.Value);
            }

            foreach (var o in other.Objects)
            {
                if (this.objects.ContainsKey(o.Name))
                {
                    this.objects.Add(o.Name, o);
                }
                else
                {
                    this.objects[o.Name].Merge(o);
                }
            }
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

        /// <summary>
        /// Gets a representation of the value of this class
        /// </summary>
        public object Value { 
            get 
            {
                return "Class " + this.Name;
            }
        }
    }
}
