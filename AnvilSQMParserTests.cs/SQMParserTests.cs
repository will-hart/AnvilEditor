using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser;
using AnvilParser.Grammar;

namespace AnvilParser.Test
{
    [TestClass]
    class SQMParserTests
    {
        [TestMethod]
        public void AnIdentifierIsASequenceOfCharacters()
        {
            var input = "name";
            var id = SQMGrammar.Identifier;
            Assert.AreEqual("name", id);
        }
    }
}
