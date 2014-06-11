﻿using System;
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
        /// An object representation of the value of the token
        /// </summary>
        object Value { get; }

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

        /// <summary>
        /// Injects a token int
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        void Inject(string path, IParserToken token);
    }
}
