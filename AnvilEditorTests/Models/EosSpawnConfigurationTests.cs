using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnvilEditor.Models;
using NUnit.Framework;
namespace AnvilEditor.Models.Tests
{
    [TestFixture()]
    public class EosSpawnConfigurationTests
    {
        [Test()]
        public void TestCovnertingConfigurationToString()
        {
            var e = new EosSpawnConfiguration();
            e.InfantryPool.Add("a1");
            e.InfantryPool.Add("a2");
            e.ArmourPool.Add("b"); 
            e.MotorisedPool.Add("c"); 
            e.AttackHeliPool.Add("d"); 
            e.HeliPool.Add("e");
            e.UavPool.Add("f");
            e.StaticPool.Add("g");
            e.ShipPool.Add("h");
            e.DiverPool.Add("i");
            e.CrewPool.Add("j");
            e.HeliCrewPool.Add("k");

            var result = e.ToString();
            var expected = "[\r\n\t[\"a1\",\"a2\"],\r\n\t[\"b\"],\r\n\t[\"c\"],\r\n\t[\"d\"],\r\n\t[\"e\"],\r\n\t[\"f\"],\r\n\t[\"g\"],\r\n\t[\"h\"],\r\n\t[\"i\"],\r\n\t[\"j\"],\r\n\t[\"k\"]\r\n]";

            Assert.AreEqual(expected, result);
        }
    }
}
