namespace AnvilEditor.Models.Tests
{
    using NUnit.Framework;
    using System;

    [TestFixture()]
    public class BriefingTests
    {
        [Test()]
        public void ToStringTest()
        {
            var b = new Briefing();
            var expected = "waitUntil { !isNil {player} };waitUntil { player == player };player createDiaryRecord [\"Diary\", [\"test\", \"other\"]];";
            b.Set("test", "other");

            Assert.AreEqual(expected, b.ToString().Replace("\r", "").Replace("\n", ""));
        }

        [Test()]
        public void DeleteByKeyShouldRemoveAnItem()
        {
            var b = new Briefing();

            b.Set("test", "other");
            var initCount = b.BriefingParts.Count;

            b.Delete("test");
            var finalCount = b.BriefingParts.Count;

            Assert.AreEqual(1, initCount);
            Assert.AreEqual(0, finalCount);
            Assert.IsFalse(b.BriefingParts.ContainsKey("test"));
            Assert.IsFalse(b.BriefingSections.Contains("test"));
        }

        [Test()]
        public void DeleteByUnknownKeyShouldLeaveCollectionUnchanged()
        {
            var b = new Briefing();

            b.Set("test", "other");
            var initCount = b.BriefingParts.Count;

            b.Delete("test22");
            var finalCount = b.BriefingParts.Count;

            Assert.AreEqual(1, initCount);
            Assert.AreEqual(1, finalCount);
            Assert.IsTrue(b.BriefingParts.ContainsKey("test"));
            Assert.IsTrue(b.BriefingSections.Contains("test"));
        }

        [Test()]
        public void SettingOnUnknownKeyShouldCreateNewSection()
        {
            var b = new Briefing();

            b.Set("test", "other");
            var initCount = b.BriefingParts.Count;

            Assert.AreEqual(1, initCount);
            Assert.IsTrue(b.BriefingParts.ContainsKey("test"));
            Assert.IsTrue(b.BriefingSections.Contains("test"));
            Assert.AreEqual("other", b.BriefingParts["test"]);
        }

        [Test()]
        public void SettingOnKnownKeyShouldUpdateExistingSection()
        {
            var b = new Briefing();

            b.Set("test", "other");
            var initCount = b.BriefingParts.Count;
            b.Set("test", "second");
            var finalCount = b.BriefingParts.Count;

            Assert.AreEqual(1, initCount);
            Assert.AreEqual(1, finalCount);
            Assert.IsTrue(b.BriefingParts.ContainsKey("test"));
            Assert.IsTrue(b.BriefingSections.Contains("test"));
            Assert.AreEqual("second", b.BriefingParts["test"]);
        }

        [Test()]
        public void GetByKeyShouldReturnEmptyStringOnUnknownKey()
        {
            var b = new Briefing();
            b.Set("test", "other");
            Assert.AreEqual(string.Empty, b.Get("ASDF"));
        }

        [Test()]
        public void GetByKeyShouldReturnDataOnKnownKey()
        {
            var b = new Briefing();
            b.Set("test", "other");
            Assert.AreEqual("other", b.Get("test"));
        }
    }
}
