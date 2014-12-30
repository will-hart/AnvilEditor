namespace AnvilEditor.Models.Tests
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    
    [TestFixture()]
    public class MissionTests
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            AnvilEditor.MainWindow.ScreenXMax = 300;
            AnvilEditor.MainWindow.ScreenYMax = 300;
            AnvilEditor.MainWindow.MapXMin = 0;
            AnvilEditor.MainWindow.MapXMax = 100;
            AnvilEditor.MainWindow.MapYMin = 0;
            AnvilEditor.MainWindow.MapYMax = 100;
        }

        [Test()]
        public void DeleteObjectiveByIdShouldRemoveObjective()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            mission.Objectives.Add(new Objective(1, new Point(1, 2)));
            var initObjCount = mission.Objectives.Count;

            mission.DeleteObjective(1);
            var finalObjCount = mission.Objectives.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void DeleteObjectiveByObjectShouldRemoveObjective()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            var obj = new Objective(1, new Point(1, 2));

            mission.Objectives.Add(obj);
            var initObjCount = mission.Objectives.Count;

            mission.DeleteObjective(obj);
            var finalObjCount = mission.Objectives.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void DeleteObjectiveShouldRemoveDeletedObjectiveFromPrereqs()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            var obj1 = new Objective(1, new Point(1, 2));
            var obj2 = new Objective(2, new Point(30, 30));
            mission.Objectives.Add(obj1);
            mission.Objectives.Add(obj2);
            
            obj2.Prerequisites.Add(1);
            var initObjCount = obj2.Prerequisites.Count;

            mission.DeleteObjective(obj1);
            var finalObjCount = obj2.Prerequisites.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void AddObjectiveShouldCreateAnObjectiveAndAddItToMission()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            var obj = mission.AddObjective(new Point(150, 225));

            Assert.AreEqual(1, mission.Objectives.Count);
            Assert.IsInstanceOf<Objective>(obj);
            Assert.AreEqual(0, obj.Id);
            Assert.AreEqual(50, obj.X);
            Assert.AreEqual(25, obj.Y);
        }

        [Test()]
        public void UseScriptShouldAddUnusedScript()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            var initScriptCount = mission.IncludedScripts.Count;
            mission.UseScript("testScript");
            var finalScriptCount = mission.IncludedScripts.Count;

            Assert.AreEqual(0, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.Contains("testScript", mission.IncludedScripts);
        }

        [Test()]
        public void UseScriptShouldntAddUsedScript()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            mission.UseScript("testScript");
            var initScriptCount = mission.IncludedScripts.Count;
            mission.UseScript("testScript");
            var finalScriptCount = mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.Contains("testScript", mission.IncludedScripts);
        }

        [Test()]
        public void UseScriptShouldntAddUnknownScript()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            var initScriptCount = mission.IncludedScripts.Count;
            mission.UseScript("testScript22");
            var finalScriptCount = mission.IncludedScripts.Count;

            Assert.AreEqual(0, initScriptCount);
            Assert.AreEqual(0, finalScriptCount);
            Assert.IsFalse(mission.IncludedScripts.Contains("testScript22"));
            Assert.IsFalse(mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void RemoveScriptShouldRemoveIncludedScriptByName()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            mission.UseScript("testScript");
            var initScriptCount = mission.IncludedScripts.Count;
            mission.RemoveScript("testScript");
            var finalScriptCount = mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(0, finalScriptCount);
            Assert.IsFalse(mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void RemoveScriptShouldntRemoveNotIncludedScriptName()
        {
            var missionMock = new Mock<Mission>(new List<AmmoboxItem>());
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            var mission = missionMock.Object;
            mission.UseScript("testScript");
            var initScriptCount = mission.IncludedScripts.Count;
            mission.RemoveScript("testScript22");
            var finalScriptCount = mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.IsTrue(mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void SettingRespawnShouldUpdateMissionWithCanvasCoordinates()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            mission.SetRespawn(new Point(150, 225));

            Assert.AreEqual(50, mission.RespawnX);
            Assert.AreEqual(25, mission.RespawnY); // screen Y coords count different direction to map coords
        }

        [Test()]
        public void SetAmbientZoneShouldCreateANewAmbientZoneAtTheGivenLocation()
        {
            var mission = new Mission(new List<AmmoboxItem>());

            var initZoneCount = mission.AmbientZones.Count;
            var zone = mission.SetAmbientZone(new Point(150, 225));
            var finalZoneCount = mission.AmbientZones.Count;

            Assert.AreEqual(0, initZoneCount);
            Assert.AreEqual(1, finalZoneCount);

            Assert.AreEqual(50, zone.X);
            Assert.AreEqual(25, zone.Y);
            Assert.AreEqual(0, zone.Id);
        }

        [Test()]
        public void SetAmbientZoneShouldIncrementId()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            var z0 = mission.SetAmbientZone(new Point());
            var z1 = mission.SetAmbientZone(new Point(1, 1));

            Assert.AreEqual(0, z0.Id);
            Assert.AreEqual(1, z1.Id);
        }

        [Test()]
        public void DeleteAmbientZonesTest()
        {
            var mission = new Mission(new List<AmmoboxItem>());
            var z0 = mission.SetAmbientZone(new Point());
            var initZoneCount = mission.AmbientZones.Count;

            mission.DeleteAmbientZone(z0);
            var finalZoneCount = mission.AmbientZones.Count;

            Assert.AreEqual(1, initZoneCount);
            Assert.AreEqual(0, finalZoneCount);
        }

        [Test()]
        public void UpdateFromSQMTest()
        {
            Assert.Ignore();
        }

        [Test()]
        public void UpdateSQMTest()
        {
            Assert.Ignore();
        }
    }
}
