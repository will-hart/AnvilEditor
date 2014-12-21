﻿namespace AnvilEditor.Tests
{
    using NUnit.Framework;
    using System.Windows;

    using AnvilEditor.Models;

    [TestFixture()]
    public class OutputGeneratorTests
    {
        [Test()]
        public void ExportTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CompleteChecksShouldIdentifyOverPopulatedObjectives()
        {
            var m = new Mission();
            var o = m.AddObjective(new Point(10, 10));
            o.Infantry = 1000;
            o.Radius = 50;

            var result = OutputGenerator.CompleteChecks(m);

            Assert.IsTrue(result.Contains("Occupation of objective 0"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifySameFriendlyAndEnemySide()
        {
            var m = new Mission();
            m.FriendlySide = "EAST";
            m.EnemySide = "EAST";
            var result = OutputGenerator.CompleteChecks(m);

            Assert.IsTrue(result.Contains("The friendly and enemy side are the same"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifyUnoccupiedObjectives()
        {
            var m = new Mission();
            var o = m.AddObjective(new Point(10, 10));
            var result = OutputGenerator.CompleteChecks(m);

            Assert.IsTrue(result.Contains("There are 1 unoccupied objective"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifyUnoccupiedAmbientZones()
        {
            var m = new Mission();
            var z = m.SetAmbientZone(new Point(1, 1));
            var result = OutputGenerator.CompleteChecks(m);

            Assert.IsTrue(result.Contains("There are 1 unoccupied ambient zone"));
        }
    }
}
