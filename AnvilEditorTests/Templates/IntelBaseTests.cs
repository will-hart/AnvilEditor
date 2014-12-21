using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnvilEditor.Templates;
using NUnit.Framework;
namespace AnvilEditor.Templates.Tests
{
    [TestFixture()]
    public class IntelBaseTests
    {
        [Test()]
        public void IntelBaseTest()
        {
            var ib = new IntelBase();

            Assert.NotNull(ib.GetToken("timeOfChanges"));
            Assert.NotNull(ib.GetToken("startWeather"));
            Assert.NotNull(ib.GetToken("startWind"));
            Assert.NotNull(ib.GetToken("startWaves"));
            Assert.NotNull(ib.GetToken("forecastWeather"));
            Assert.NotNull(ib.GetToken("forecastWind"));
            Assert.NotNull(ib.GetToken("forecastWaves"));
            Assert.NotNull(ib.GetToken("forecastLightnings"));
            Assert.NotNull(ib.GetToken("forecastFogDecay"));
            Assert.NotNull(ib.GetToken("year"));
            Assert.NotNull(ib.GetToken("month"));
            Assert.NotNull(ib.GetToken("day"));
            Assert.NotNull(ib.GetToken("hour"));
            Assert.NotNull(ib.GetToken("minute"));
        }
    }
}
