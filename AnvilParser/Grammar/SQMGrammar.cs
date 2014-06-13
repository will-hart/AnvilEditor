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

        public static readonly Parser<char> SemiParser = Parse.Char(';').Token();
        public static readonly Parser<char> CommaParser = Parse.Char(',').Token();
        public static readonly Parser<string> NewLineParser = Parse.String(Environment.NewLine).Text().Token();

        /// <summary>
        /// Reads in a positive or negative decimal number
        /// </summary>
        public static readonly Parser<decimal> DecimalParser = 
            from negative in Parse.Char('-').Optional().Token()
            from number in Parse.Decimal.Select(decimal.Parse).Token()
            select negative.IsDefined ? -1.0m * number : number;

        /// <summary>
        /// Reads in a positive or negative integer
        /// </summary>
        public static readonly Parser<int> IntParser = 
            from negative in Parse.Char('-').Optional().Token()
            from number in Parse.Number.Select(int.Parse).Token()
            select negative.IsDefined ? -1 * number : number;

        /// <summary>
        /// Gets the identifier from a statement - e.g. in id = 3; the identifier is 'id'
        /// </summary>
        public static readonly Parser<string> Identifier =
            from leading in Parse.WhiteSpace.Many()
            from n1 in Parse.Letter.AtLeastOnce().Text()
            from name in Parse.LetterOrDigit.Many().Text().Token()
            from eq in Parse.Char('=').Token()
            select n1 + name;

        /// <summary>
        /// Selects the identifier for an array declaration e.g. my_array[] = {} will give my_array
        /// </summary>
        public static readonly Parser<string> ArrayIdentifier =
            from leading in Parse.WhiteSpace.Many()
            from n1 in Parse.Letter.AtLeastOnce().Text()
            from name in Parse.LetterOrDigit.Many().Text().Token()
            from arrOpen in Parse.Char('[').Once()
            from arrClose in Parse.Char(']').Once()
            from eq in Parse.Char('=').Token()
            from nl in NewLineParser.Optional()
            select n1 + name;

        /// <summary>
        /// Parses the opening lines of a class - "class ClassName {" and returns ClassName
        /// </summary>
        public static readonly Parser<string> ClassIdentifier =
            from leading in Parse.WhiteSpace.Many()
            from cls in Parse.String("class").AtLeastOnce().Token()
            from n1 in Parse.Letter.AtLeastOnce().Text().Token()
            from name in Parse.LetterOrDigit.Many().Text().Token()
            from nl in NewLineParser.Optional()
            select n1 + name;

        /// <summary>
        /// Parses a quoted string, e.g. "test" with any amount of white space surrounding it
        /// 
        /// Substring should always work without a bounds check as if it isn't at least two characters '""' 
        /// it shouldn't have matched the QuotedText parser
        /// </summary>
        public static readonly Parser<string> QuotedText =
            from content in Parse.Regex(@"""(?:""""|[^""])*""").Text()
            select (content).Substring(1, content.Length - 2);

        /// <summary>
        /// Handles a string assignment operator e.g. id = "3";
        /// </summary>
        public static readonly Parser<ParserObject> StringObjectParser =
            from name in Identifier
            from value in QuotedText
            from endsemi in SemiParser
            select new ParserObject(name) { Value = value };

        /// <summary>
        /// Handles an integer assignment operator - e.g. "id = 3;
        /// </summary>
        public static readonly Parser<ParserObject> IntObjectParser =
            from name in Identifier
            from value in IntParser
            from endsemi in SemiParser
            select new ParserObject(name) { Value = value };

        /// <summary>
        /// Handles an integer assignment operator - e.g. "id = 3;
        /// </summary>
        public static readonly Parser<ParserObject> DecimalObjectParser =
            from name in Identifier
            from value in DecimalParser
            from endsemi in SemiParser
            select new ParserObject(name) { Value = value };

        /// <summary>
        /// Returns a single type of object back to the caller
        /// </summary>
        public static readonly Parser<ParserObject> ObjectParser = 
            StringObjectParser.Or(
                DecimalObjectParser.Or(
                    IntObjectParser
                )
            );

        /// <summary>
        /// Parses an array item which may have a trailing comma and newline
        /// </summary>
        public static readonly Parser<object> ArrayItem =
            from obj in QuotedText.Select(o => (object)o).Or(
                DecimalParser.Select(o => (object)o).Or(
                    IntParser.Select(o => (object)o)
                )
            )
            from comma in CommaParser.Optional()
            from newline in NewLineParser.Optional()
            select (object)obj;

        /// <summary>
        /// Parses an array of objects
        /// </summary>
        public static readonly Parser<ParserArray> ArrayParser =
            from name in ArrayIdentifier
            from arrOpen in Parse.Char('{').Once().Token()
            from items in ArrayItem.Many()
            from arrClose in Parse.Char('}').Once().Token()
            from endsemi in SemiParser
            select new ParserArray(name) { Items = items.ToList() };

        /// <summary>
        /// Select a single token, either an array or an object
        /// </summary>
        public static readonly Parser<IParserToken> TokenParser =
            from tok in ObjectParser.Select(o => (IParserToken)o).Or(
                ArrayParser.Select(o => (IParserToken)o)
            )
            select tok;

        /// <summary>
        /// Parses a class, which may include a number of tokens or sub classes
        /// </summary>
        public static readonly Parser<ParserClass> ClassParser =
            from name in ClassIdentifier
            from clsOpen in Parse.Char('{').Once().Token()
            from toks in TokenParser.Many()
            from cls in ClassParser.Many()
            from clsClose in Parse.Char('}').Once().Token()
            from endsemi in SemiParser.Optional()
            select new ParserClass(name, toks.ToList(), cls.ToList());

        /// <summary>
        /// Parses an entire document, returning a MissionWrapper class
        /// </summary>
        public static readonly Parser<ParserClass> SQMParser =
            from toks in TokenParser.Many()
            from objs in ClassParser.Many()
            select new ParserClass("root") { Tokens = toks.ToList(), Objects = objs.ToList() };
    }
}
