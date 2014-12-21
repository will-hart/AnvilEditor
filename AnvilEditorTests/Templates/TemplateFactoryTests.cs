namespace AnvilEditor.Templates.Tests
{
    using NUnit.Framework;

    using AnvilEditor.Templates;
    using AnvilParser;
    using AnvilEditor.Models;
    using System.Collections.Generic;

    [TestFixture()]
    public class TemplateFactoryTests
    {
        [Test()]
        public void MissionTemplateShouldGenerateRootMissionBase()
        {
            var tpl = TemplateFactory.Mission();

            Assert.NotNull(tpl.GetClass("Mission"));
            Assert.NotNull(tpl.GetClass("Mission.Groups"));
            Assert.NotNull(tpl.GetClass("Mission.Intel"));
            Assert.NotNull(tpl.GetToken("Mission.Addons"));
            Assert.NotNull(tpl.GetToken("Mission.addOnsAuto"));
            Assert.NotNull(tpl.GetToken("Mission.randomSeed"));

            Assert.NotNull(tpl.GetClass("Intro"));
            Assert.NotNull(tpl.GetClass("Intro.Intel"));
            Assert.NotNull(tpl.GetToken("Intro.Addons"));
            Assert.NotNull(tpl.GetToken("Intro.addOnsAuto"));
            Assert.NotNull(tpl.GetToken("Intro.randomSeed"));

            Assert.NotNull(tpl.GetClass("OutroLoose"));
            Assert.NotNull(tpl.GetClass("OutroLoose.Intel"));
            Assert.NotNull(tpl.GetToken("OutroLoose.Addons"));
            Assert.NotNull(tpl.GetToken("OutroLoose.addOnsAuto"));
            Assert.NotNull(tpl.GetToken("OutroLoose.randomSeed"));

            Assert.NotNull(tpl.GetClass("OutroWin"));
            Assert.NotNull(tpl.GetClass("OutroWin.Intel"));
            Assert.NotNull(tpl.GetToken("OutroWin.Addons"));
            Assert.NotNull(tpl.GetToken("OutroWin.addOnsAuto"));
            Assert.NotNull(tpl.GetToken("OutroWin.randomSeed"));
        }

        [Test()]
        public void GetTemplateTest()
        {
            Assert.Ignore();
        }

        [Test()]
        public void MarkerTemplateAppliesCustomValues()
        {
            var mkr_defaults = TemplateFactory.Marker(10, 20, "test_marker", "ColorRed", "test marker");
            
            Assert.AreEqual("Item000_test_marker", mkr_defaults.Name);
            Assert.AreEqual("[ 10, 0.0, 20]", mkr_defaults.GetToken("position").ToString());
            Assert.AreEqual("\"test_marker\"", mkr_defaults.GetToken("name").ToString());
            Assert.AreEqual("\"Empty\"", mkr_defaults.GetToken("type").ToString());
            Assert.AreEqual("\"ColorRed\"", mkr_defaults.GetToken("colorName").ToString());
            Assert.AreEqual("\"test marker\"", mkr_defaults.GetToken("text").ToString());
        }

        [Test()]
        public void MarkerSetsCorrectDefaults()
        {
            var mkr_defaults = TemplateFactory.Marker(10, 20, "test_marker");


            Assert.AreEqual("Item000_test_marker", mkr_defaults.Name);
            Assert.AreEqual("[ 10, 0.0, 20]", mkr_defaults.GetToken("position").ToString());
            Assert.AreEqual("\"test_marker\"", mkr_defaults.GetToken("name").ToString());
            Assert.AreEqual("\"Empty\"", mkr_defaults.GetToken("type").ToString());
            Assert.AreEqual("\"ColorOrange\"", mkr_defaults.GetToken("colorName").ToString());
            Assert.AreEqual("\"\"", mkr_defaults.GetToken("text").ToString());
        }

        [Test()]
        public void CompleteObjectiveTriggerTest()
        {
            var aot = TemplateFactory.CompleteObjectiveTrigger(1, EndTriggerTypes.END2);

            var expectedCondition = "server getVariable \"\"objective_1\"\"";

            Assert.AreEqual(aot.Name, "Item0_000_1");
            Assert.AreEqual(aot.GetToken("type").Value.ToString(), "END2");
            Assert.AreEqual(aot.GetToken("name").Value.ToString(), "fw_trig_obj1");
            Assert.AreEqual(aot.GetToken("expCond").Value.ToString(), expectedCondition);
            Assert.AreEqual(aot.GetToken("timeoutMin").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMid").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMax").Value.ToString(), "10");
        }

        [Test()]
        public void AllObjectivesTriggerShouldBuildCorrectParserClass()
        {
            var aot = TemplateFactory.AllObjectivesTrigger(EndTriggerTypes.END1);

            Assert.AreEqual(aot.Name, "Item0_000_all");
            Assert.AreEqual(aot.GetToken("type").Value.ToString(), "END1");
            Assert.AreEqual(aot.GetToken("name").Value.ToString(), "fw_trig_obj_all");
            Assert.AreEqual(aot.GetToken("expCond").Value.ToString(), "all_objectives_complete");
            Assert.AreEqual(aot.GetToken("timeoutMin").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMid").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMax").Value.ToString(), "10");
        }

        [Test()]
        public void KeyObjectivesTriggerTest()
        {
            var objs = new List<Objective>()
            {
                new Objective(1, new System.Windows.Point(0, 0)),
                new Objective(2, new System.Windows.Point(0, 0))
            };
            var aot = TemplateFactory.KeyObjectivesTrigger(objs, EndTriggerTypes.END1);

            var expectedCondition = "(server getVariable \"\"objective_1\"\") and (server getVariable \"\"" +
                "objective_2\"\")";

            Assert.AreEqual(aot.Name, "Item0_000_key");
            Assert.AreEqual(aot.GetToken("type").Value.ToString(), "END1");
            Assert.AreEqual(aot.GetToken("name").Value.ToString(), "fw_trig_obj_key");
            Assert.AreEqual(aot.GetToken("expCond").Value.ToString(), expectedCondition);
            Assert.AreEqual(aot.GetToken("timeoutMin").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMid").Value.ToString(), "10");
            Assert.AreEqual(aot.GetToken("timeoutMax").Value.ToString(), "10");
        }

        [Test()]
        public void LoadAllTemplatesTest()
        {
            Assert.Ignore();
        }
    }
}
