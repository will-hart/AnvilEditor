﻿using System;
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

        public ParserClass(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns the SQM text for this token
        /// </summary>
        /// <returns></returns>
        public string ToSQM()
        {
            return @"class " + this.Name + @"{}";
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

        private ParserObject Add(string name, object value) {
            
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

        public ParserObject Add(string name, int value)
        {
            return this.Add(name, value);
        }

        public ParserObject Add(string name, double value)
        {
            return this.Add(name, value);
        }

        public ParserObject Add(string name, string value)
        {
            return this.Add(name, value);
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
        /// The name of the element
        /// </summary>
        public string Name { get; set; }
    }
}
