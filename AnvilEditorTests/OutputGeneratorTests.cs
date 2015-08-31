namespace AnvilEditor.Tests
{
    using NUnit.Framework;
    using System.Windows;

    using AnvilEditor.Helpers;
    using AnvilEditor.Models;
    using System.Collections.Generic;
    using Moq;

    [TestFixture()]
    public class OutputHelperTests
    {
        [Test()]
        public void ExportTest()
        {
            Assert.Ignore();
        }

        [Test()]
        public void CompleteChecksShouldIdentifyOverPopulatedObjectives()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() {
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            var o = mission.AddObjective(new Point(10, 10));
            o.Infantry = 1000;
            o.Radius = 50;

            var result = OutputHelper.CompleteChecks(mission);

            Assert.IsTrue(result.Contains("Occupation of objective 0"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifySameFriendlyAndEnemySide()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() {
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;

            mission.FriendlySide = "EAST";
            mission.EnemySide = "EAST";
            var result = OutputHelper.CompleteChecks(mission);

            Assert.IsTrue(result.Contains("The friendly and enemy side are the same"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifyUnoccupiedObjectives()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() {
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            var o = mission.AddObjective(new Point(10, 10));
            var result = OutputHelper.CompleteChecks(mission);

            Assert.IsTrue(result.Contains("There are 1 unoccupied objective"));
        }

        [Test()]
        public void CompleteChecksShouldIdentifyUnoccupiedAmbientZones()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() {
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            var z = mission.SetAmbientZone(new Point(1, 1));
            var result = OutputHelper.CompleteChecks(mission);

            Assert.IsTrue(result.Contains("There are 1 unoccupied ambient zone"));
        }
    }
}
