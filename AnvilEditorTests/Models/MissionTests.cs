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
        private Mission mission;

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
            this.mission = new Mission();
            this.mission.Objectives.Add(new Objective(1, new Point(1, 2)));
            var initObjCount = this.mission.Objectives.Count;

            this.mission.DeleteObjective(1);
            var finalObjCount = this.mission.Objectives.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void DeleteObjectiveByObjectShouldRemoveObjective()
        {
            this.mission = new Mission();
            var obj = new Objective(1, new Point(1, 2));

            this.mission.Objectives.Add(obj);
            var initObjCount = this.mission.Objectives.Count;

            this.mission.DeleteObjective(obj);
            var finalObjCount = this.mission.Objectives.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void DeleteObjectiveShouldRemoveDeletedObjectiveFromPrereqs()
        {
            this.mission = new Mission();
            var obj1 = new Objective(1, new Point(1, 2));
            var obj2 = new Objective(2, new Point(30, 30));
            this.mission.Objectives.Add(obj1);
            this.mission.Objectives.Add(obj2);
            
            obj2.Prerequisites.Add(1);
            var initObjCount = obj2.Prerequisites.Count;

            this.mission.DeleteObjective(obj1);
            var finalObjCount = obj2.Prerequisites.Count;

            Assert.AreEqual(1, initObjCount);
            Assert.AreEqual(0, finalObjCount);
        }

        [Test()]
        public void AddObjectiveShouldCreateAnObjectiveAndAddItToMission()
        {
            this.mission = new Mission();
            var obj = this.mission.AddObjective(new Point(150, 225));

            Assert.AreEqual(1, this.mission.Objectives.Count);
            Assert.IsInstanceOf<Objective>(obj);
            Assert.AreEqual(0, obj.Id);
            Assert.AreEqual(50, obj.X);
            Assert.AreEqual(25, obj.Y);
        }

        [Test()]
        public void UseScriptShouldAddUnusedScript()
        {
            var missionMock = new Mock<Mission>();
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            this.mission = missionMock.Object;
            var initScriptCount = this.mission.IncludedScripts.Count;
            this.mission.UseScript("testScript");
            var finalScriptCount = this.mission.IncludedScripts.Count;

            Assert.AreEqual(0, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.Contains("testScript", this.mission.IncludedScripts);
        }

        [Test()]
        public void UseScriptShouldntAddUsedScript()
        {
            var missionMock = new Mock<Mission>();
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            this.mission = missionMock.Object;
            this.mission.UseScript("testScript");
            var initScriptCount = this.mission.IncludedScripts.Count;
            this.mission.UseScript("testScript");
            var finalScriptCount = this.mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.Contains("testScript", this.mission.IncludedScripts);
        }

        [Test()]
        public void UseScriptShouldntAddUnknownScript()
        {
            var missionMock = new Mock<Mission>();
            missionMock.SetupGet(m => m.AvailableScripts).Returns(new List<ScriptInclude>() { 
                new ScriptInclude() {
                    FriendlyName = "testScript"
                }
            });

            this.mission = missionMock.Object;
            var initScriptCount = this.mission.IncludedScripts.Count;
            this.mission.UseScript("testScript22");
            var finalScriptCount = this.mission.IncludedScripts.Count;

            Assert.AreEqual(0, initScriptCount);
            Assert.AreEqual(0, finalScriptCount);
            Assert.IsFalse(this.mission.IncludedScripts.Contains("testScript22"));
            Assert.IsFalse(this.mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void RemoveScriptShouldRemoveIncludedScriptByName()
        {
            this.mission = new Mission();
            this.mission.UseScript("testScript");
            var initScriptCount = this.mission.IncludedScripts.Count;
            this.mission.RemoveScript("testScript");
            var finalScriptCount = this.mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(0, finalScriptCount);
            Assert.IsFalse(this.mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void RemoveScriptShouldntRemoveNotIncludedScriptName()
        {
            this.mission = new Mission();
            this.mission.UseScript("testScript");
            var initScriptCount = this.mission.IncludedScripts.Count;
            this.mission.RemoveScript("testScript22");
            var finalScriptCount = this.mission.IncludedScripts.Count;

            Assert.AreEqual(1, initScriptCount);
            Assert.AreEqual(1, finalScriptCount);
            Assert.IsTrue(this.mission.IncludedScripts.Contains("testScript"));
        }

        [Test()]
        public void SettingRespawnShouldUpdateMissionWithCanvasCoordinates()
        {
            this.mission = new Mission();
            this.mission.SetRespawn(new Point(150, 225));

            Assert.AreEqual(50, this.mission.RespawnX);
            Assert.AreEqual(25, this.mission.RespawnY); // screen Y coords count different direction to map coords
        }

        [Test()]
        public void SetAmbientZoneShouldCreateANewAmbientZoneAtTheGivenLocation()
        {
            this.mission = new Mission();

            var initZoneCount = this.mission.AmbientZones.Count;
            var zone = this.mission.SetAmbientZone(new Point(150, 225));
            var finalZoneCount = this.mission.AmbientZones.Count;

            Assert.AreEqual(0, initZoneCount);
            Assert.AreEqual(1, finalZoneCount);

            Assert.AreEqual(50, zone.X);
            Assert.AreEqual(25, zone.Y);
            Assert.AreEqual(0, zone.Id);
        }

        [Test()]
        public void SetAmbientZoneShouldIncrementId()
        {
            this.mission = new Mission();
            var z0 = this.mission.SetAmbientZone(new Point());
            var z1 = this.mission.SetAmbientZone(new Point(1, 1));

            Assert.AreEqual(0, z0.Id);
            Assert.AreEqual(1, z1.Id);
        }

        [Test()]
        public void DeleteAmbientZonesTest()
        {
            this.mission = new Mission();
            var z0 = this.mission.SetAmbientZone(new Point());
            var initZoneCount = this.mission.AmbientZones.Count;

            this.mission.DeleteAmbientZone(z0);
            var finalZoneCount = this.mission.AmbientZones.Count;

            Assert.AreEqual(1, initZoneCount);
            Assert.AreEqual(0, finalZoneCount);
        }

        [Test()]
        public void UpdateFromSQMTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void UpdateSQMTest()
        {
            Assert.Fail();
        }
    }
}
