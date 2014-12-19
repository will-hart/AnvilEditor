using AnvilEditor.Models;
using NUnit.Framework;

namespace AnvilEditor.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using AnvilEditor.Models;
    using NUnit.Framework;

    [TestFixture()]
    public class ObjectiveBaseTests
    {
        [Test()]
        public void MapToCanvasXTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CanvasToMapXTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void MapToCanvasYTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CanvasToMapYTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateMarkerTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsOccupiedShouldChangeWhenTroopsAreAdded()
        {
            var b = new ObjectiveBase(1, new Point(2.0, 3.0));
            Assert.IsFalse(b.IsOccupied);

            b.Air = 1;
            Assert.IsTrue(b.IsOccupied);
        }
    }
}
