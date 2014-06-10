using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;
using Sprache;

namespace AnvilParser.Grammar
{
    /// <summary>
    /// A grammar definition for SQM files based on http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/
    /// and the Sprache library https://github.com/sprache/Sprache
    /// </summary>
    public class SQMGrammar
    {
        /// <summary>
        /// Gets the identifier from a statement - e.g. in id = 3; the identifier is 'id'
        /// </summary>
        public static readonly Parser<string> Identifier =
                from leading in Parse.WhiteSpace.Many()
                from name in Parse.LetterOrDigit.Many().Text().Token()
                from eq in Parse.Char('=').Token()
                select name;

        /// <summary>
        /// Selects the identifier for an array declaration e.g. my_array[] = {} will give my_array
        /// </summary>
        public static readonly Parser<string> ArrayIdentifier = 
                from leading in Parse.WhiteSpace.Many()
                from name in Parse.CharExcept('[').Many().Text().Token()
                from arrOpen in Parse.Char('[').Once()
                from arrClose in Parse.Char(']').Once()
                from eq in Parse.Char('=').Token()
                select name;


        /// <summary>
        /// Parses a quoted string, e.g. "test" with any amount of white space surrounding it
        /// </summary>
        public static readonly Parser<string> QuotedText =
            from leading in Parse.WhiteSpace.Many()
            from open in Parse.Char('"')
            from content in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"').Token()
            select content;

        /// <summary>
        /// Handles a string assignment operator e.g. id = "3";
        /// </summary>
        public static readonly Parser<ParserObject> StringObjectParser =
                from name in Identifier
                from value in QuotedText
                from endsemi in Parse.Char(';').Token()
                select new ParserObject() { Name = name, Value = value };

        /// <summary>
        /// Handles an integer assignment operator - e.g. "id = 3;
        /// </summary>
        public static readonly Parser<ParserObject> IntObjectParser =
                from name in Identifier
                from value in Parse.Number.Select(int.Parse).Token()
                from endsemi in Parse.Char(';').Token()
                select new ParserObject() { Name = name, Value = value };

        /// <summary>
        /// Handles an integer assignment operator - e.g. "id = 3;
        /// </summary>
        public static readonly Parser<ParserObject> FloatObjectParser =
                from name in Identifier
                from value in Parse.Decimal.Select(decimal.Parse).Token()
                from endsemi in Parse.Char(';').Token()
                select new ParserObject() { Name = name, Value = value };
    }
}
