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
    public class ObjectiveTests
    {
        [Test()]
        public void AddPrerequisiteShouldAddIdToList()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            o.AddPrerequisite(3);

            Assert.AreEqual(1, o.Prerequisites.Count());
            Assert.Contains(3, o.Prerequisites);
        }

        [Test()]
        public void AddExistingPrerequisiteShouldntAddIdToList()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            o.AddPrerequisite(3);

            Assert.AreEqual(1, o.Prerequisites.Count());
            Assert.Contains(3, o.Prerequisites);

            o.AddPrerequisite(3);
            Assert.AreEqual(1, o.Prerequisites.Count());
            Assert.Contains(3, o.Prerequisites);
        }

        [Test()]
        public void AmmoMarkerTextShouldBeEmptyOnNoMarker()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            Assert.AreEqual(string.Empty, o.AmmoMarker);
        }

        [Test()]
        public void AmmoMarkerTextShouldBeValidWhenMarkerRequired()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            o.Ammo = true;

            Assert.AreEqual("ammo_1", o.AmmoMarker);
        }

        [Test()]
        public void SpecialMarkerTextShouldBeEmptyOnNoMarker()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            Assert.AreEqual(string.Empty, o.SpecialMarker);
        }

        [Test()]
        public void SpecialMarkerTextBeValid()
        {
            var o = new Objective(1, new System.Windows.Point(1.0, 2.0));
            o.Special = true;

            Assert.AreEqual("special_1", o.SpecialMarker);
        }

        [Test()]
        public void GetInitTextTest()
        {
            Assert.Fail();
        }
    }
}
